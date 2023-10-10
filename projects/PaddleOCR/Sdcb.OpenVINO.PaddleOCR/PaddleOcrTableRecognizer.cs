using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents a PaddleOCR table recognizer.
/// </summary>
public class PaddleOcrTableRecognizer : IDisposable
{
    readonly InferRequest _p;

    /// <summary>
    /// Gets or sets the maximum edge size.
    /// </summary>
    public int MaxEdgeSize { get; set; } = 488;

    /// <summary>
    /// Gets the table recognition model.
    /// </summary>
    public TableRecognitionModel Model { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrTableRecognizer"/> class.
    /// </summary>
    /// <param name="model">The TableRecognitionModel to use for recognition.</param>
    /// <param name="deviceOptions">The optional DeviceOptions to use for inference.</param>
    public PaddleOcrTableRecognizer(TableRecognitionModel model, DeviceOptions? deviceOptions = null)
    {
        Model = model;
        _p = model.CreateInferRequest(deviceOptions);
    }

    /// <summary>
    /// Disposes the PaddleOCR table recognizer.
    /// </summary>   
    public void Dispose() => _p.Dispose();

    /// <summary>
    /// Runs table detection on the image.
    /// </summary>
    /// <param name="src">The input image to run table detection on.</param>
    /// <returns>The table detection result.</returns>
    /// <exception cref="ArgumentException">Thrown when the input image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown when the input image channel is not 3 or 1.</exception>
    /// <exception cref="Exception">Thrown when the PaddlePredictor(Table) run failed.</exception>
    public TableDetectionResult Run(Mat src)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
        {
            throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
        }

        Size rawSize = src.Size();
        float[] inputData = TablePreprocess(src);

        using (Tensor input = Tensor.FromArray(inputData, new Shape(1, 3, MaxEdgeSize, MaxEdgeSize)))
        {
            _p.Inputs.Primary = input;
        }

        _p.Run();

        using (Tensor output0 = _p.Outputs[0])
        using (Tensor output1 = _p.Outputs[1])
        {
            Span<float> locations = output0.GetData<float>();
            Shape locationShape = output0.Shape;
            Span<float> structures = output1.GetData<float>();
            Shape structureShape = output1.Shape;

            return TablePostProcessor(locations, locationShape, structures, structureShape, rawSize, Model.GetLabelByIndex);
        }
    }

    private float[] TablePreprocess(Mat src)
    {
        using Mat resized = MatResize(src, MaxEdgeSize);
        using Mat normalized = Normalize(resized);
        using Mat padded = MatPadingTo(normalized, MaxEdgeSize);
        return ExtractMat(padded);

        static Mat MatResize(Mat src, int maxSize)
        {
            int w = src.Width;
            int h = src.Height;

            float ratio = w >= h ? (float)maxSize / w : (float)maxSize / h;
            int resizeH = (int)(h * ratio);
            int resizeW = (int)(w * ratio);
            return src.Resize(new Size(resizeW, resizeH));
        }

        static float[] ExtractMat(Mat src)
        {
            int rows = src.Rows;
            int cols = src.Cols;
            float[] result = new float[rows * cols * 3];
            GCHandle resultHandle = default;
            try
            {
                resultHandle = GCHandle.Alloc(result, GCHandleType.Pinned);
                IntPtr resultPtr = resultHandle.AddrOfPinnedObject();
                for (int i = 0; i < src.Channels(); ++i)
                {
                    using Mat dest = new(rows, cols, MatType.CV_32FC1, resultPtr + i * rows * cols * sizeof(float));
                    Cv2.ExtractChannel(src, dest, i);
                }
            }
            finally
            {
                resultHandle.Free();
            }
            return result;
        }

        static Mat MatPadingTo(Mat src, int maxSize)
        {
            Size size = src.Size();
            Size newSize = new(maxSize, maxSize);
            return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, Scalar.Black);
        }

        static Mat Normalize(Mat src)
        {
            using Mat normalized = new();
            src.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
            Mat[] bgr = normalized.Split();
            float[] scales = new[] { 1 / 0.229f, 1 / 0.224f, 1 / 0.225f };
            float[] means = new[] { 0.485f, 0.456f, 0.406f };
            for (int i = 0; i < bgr.Length; ++i)
            {
                bgr[i].ConvertTo(bgr[i], MatType.CV_32FC1, 1.0 * scales[i], (0.0 - means[i]) * scales[i]);
            }

            Mat dest = new();
            Cv2.Merge(bgr, dest);

            foreach (Mat channel in bgr)
            {
                channel.Dispose();
            }

            return dest;
        }
    }

    private static TableDetectionResult TablePostProcessor(
        Span<float> locations, Shape locationShape,
        Span<float> structures, Shape structureShape,
        Size rawSize,
        Func<int, string> labelAccessor)
    {
        float score = 0.0f;
        int count = 0;
        List<string> recHtmlTags = new();
        List<TableCellBox> recBoxes = new();

        for (int stepIndex = 0; stepIndex < structureShape[1]; ++stepIndex)
        {
            List<int> recBox = new();
            string htmlTag;
            // html tag
            {
                int stepStartIndex = stepIndex * structureShape[2];
                (int charIndex, float charScore) = ArgMax(structures[stepStartIndex..(stepStartIndex + structureShape[2])]);
                htmlTag = labelAccessor(charIndex);
                if (stepIndex > 0 && htmlTag == TableRecognitionModelConsts.LastLabel)
                {
                    break;
                }
                if (htmlTag == TableRecognitionModelConsts.FirstLabel)
                {
                    continue;
                }
                count += 1;
                score += charScore;
                recHtmlTags.Add(htmlTag);
            }

            // box
            if (htmlTag == "<td>" || htmlTag == "<td" || htmlTag == "<td></td>")
            {
                for (int pointIndex = 0; pointIndex < locationShape[2]; pointIndex++)
                {
                    int stepStartIndex = stepIndex * locationShape[2] + pointIndex;
                    float point = locations[stepStartIndex];
                    if (pointIndex % 2 == 0)
                    {
                        point *= rawSize.Width;
                    }
                    else
                    {
                        point *= rawSize.Height;
                    }
                    recBox.Add((int)point);
                }
                recBoxes.Add(new TableCellBox(recBox));
            }
        }

        score /= count;
        if (float.IsNaN(score) || recBoxes.Count == 0)
        {
            score -= 1;
        }
        return new TableDetectionResult(score, recBoxes, recHtmlTags);
    }

    private static (int, float) ArgMax(ReadOnlySpan<float> collection)
    {
        int maxIndex = -1;
        float maxValue = float.NegativeInfinity;
        for (int i = 0; i < collection.Length; i++)
        {
            float value = collection[i];
            if (maxIndex == -1 || value > maxValue)
            {
                maxIndex = i;
                maxValue = value;
            }
        }
        return (maxIndex, maxValue);
    }
}
