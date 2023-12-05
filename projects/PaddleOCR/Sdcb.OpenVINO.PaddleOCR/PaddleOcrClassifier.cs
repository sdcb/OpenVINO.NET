using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;
using System;
using System.Linq;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Implements a PaddleOCR classifier using a PaddlePredictor.
/// </summary>
public class PaddleOcrClassifier : IDisposable
{
    private readonly CompiledModel _compiledModel;

    /// <summary>
    /// Rotation threshold value used to determine if the image should be rotated.
    /// </summary>
    public double RotateThreshold { get; init; } = 0.5;

    /// <summary>
    /// The OcrShape used for the model.
    /// </summary>
    public NCHW Shape { get; init; } = ClassificationModel.DefaultShape;

    /// <summary>
    /// The batch size used for inference. If not specified or zero, the batch size will be determined by minimum of 8 and the processor count.
    /// </summary>
    /// <remarks>
    /// The batch size determines the number of images that will be processed in parallel during inference. A larger batch size can lead to faster inference times, but may require more memory and may not fit on devices with limited memory.
    /// </remarks>
    public int BatchSize { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrClassifier"/> class with a specified model and configuration.
    /// </summary>
    /// <param name="model">The <see cref="ClassificationModel"/> to use.</param>
    /// <param name="device">The device the inference request, pass null to using model's DefaultDevice.</param>
    public PaddleOcrClassifier(ClassificationModel model, 
        DeviceOptions? device = null)
    {
        Shape = model.Shape;
        _compiledModel = model.CreateCompiledModel(device, prePostProcessing: (m, ppp) =>
        {
            using PreProcessInputInfo ppii = ppp.Inputs.Primary;
            ppii.TensorInfo.Layout = Layout.NHWC;
            ppii.ModelInfo.Layout = Layout.NCHW;
        });
    }

    /// <summary>
    /// Releases all resources used by the <see cref="PaddleOcrClassifier"/> object.
    /// </summary>
    public void Dispose() => _compiledModel.Dispose();

    /// <summary>
    /// Determines whether the image should be rotated by 180 degrees based on the threshold value.
    /// </summary>
    /// <param name="src">The source image.</param>
    /// <returns>An instance of the <see cref="Ocr180DegreeClsResult"/> record struct, containing a bool indicating whether the image should be rotated and a float representing the confidence of the determination.</returns>
    /// <exception cref="ArgumentException">Thrown if the source image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if the source image has a channel count other than 3 or 1.</exception>
    public Ocr180DegreeClsResult ShouldRotate180(Mat src)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
        {
            throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
        }

        return BatchedShouldRotate180(new[] { src }).Single();
    }

    /// <summary>
    /// Determines whether each image in an array should be rotated by 180 degrees based on the threshold value.
    /// </summary>
    /// <param name="srcs">The array of source images.</param>
    /// <returns>An array of Ocr180DegreeClsResult instances, each containing a bool indicating whether the corresponding image should be rotated and a float representing the confidence of the determination.</returns>
    public Ocr180DegreeClsResult[] ShouldRotate180(Mat[] srcs)
    {
        if (srcs.Length == 0)
        {
            return new Ocr180DegreeClsResult[0];
        }

        int chooseBatchSize = BatchSize != 0 ? BatchSize : Math.Min(8, Environment.ProcessorCount);
        Ocr180DegreeClsResult[] allResult = new Ocr180DegreeClsResult[srcs.Length];

        return srcs
            .Select((x, i) => (mat: x, i))
            .Chunk(chooseBatchSize)
            .Select(x => (result: BatchedShouldRotate180(x.Select(x => x.mat).ToArray()), ids: x.Select(x => x.i).ToArray()))
            .SelectMany(x => x.result.Zip(x.ids, (result, i) => (result, i)))
            .Select(x => x.result)
            .ToArray();
    }


    private Ocr180DegreeClsResult[] BatchedShouldRotate180(Mat[] srcs)
    {
        using Mat final = PrepareAndStackImages(srcs);

        using InferRequest ir = _compiledModel.CreateInferRequest();
        using (Tensor input = final.StackedAsTensor(srcs.Length))
        {
            ir.Inputs.Primary = input;
            ir.Run();
        }

        using Tensor output = ir.Outputs.Primary;
        ReadOnlySpan<float> data = output.GetData<float>();
        Ocr180DegreeClsResult[] results = new Ocr180DegreeClsResult[data.Length / 2];
        for (int i = 0; i < results.Length; i++)
        {
            results[i] = new Ocr180DegreeClsResult(data[(i * 2)..((i + 1) * 2)], RotateThreshold);
        }

        return results;
    }

    private Mat PrepareAndStackImages(Mat[] srcs)
    {
        Mat[] normalizeds = null!;
        Mat final = new();
        try
        {
            normalizeds = srcs
                .Select(src =>
                {
                    using Mat channel3 = src.Channels() switch
                    {
                        4 => src.CvtColor(ColorConversionCodes.RGBA2RGB),
                        1 => src.CvtColor(ColorConversionCodes.GRAY2RGB),
                        3 => src.FastClone(),
                        var x => throw new Exception($"Unexpect src channel: {x}, allow: (1/3/4)")
                    };
                    return ResizePadding(channel3, Shape);
                })
                .ToArray();
            using Mat combined = normalizeds.StackingVertically();
            final = Normalize(combined);
        }
        finally
        {
            foreach (Mat normalized in normalizeds)
            {
                normalized.Dispose();
            }
        }

        return final;
    }

    /// <summary>
    /// Processes the input images directly and rotates each image if necessary.
    /// </summary>
    /// <param name="srcs">The source images.</param>
    /// <exception cref="ArgumentException">Thrown if the source image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if the source image has a channel count other than 3 or 1.</exception>
    /// <remarks>No return value. The source images are modified directly.</remarks>
    public void Run(Mat[] srcs)
    {
        Ocr180DegreeClsResult[] ress = ShouldRotate180(srcs);
        for (int i = 0; i < srcs.Length; i++)
        {
            Mat src = srcs[i];
            ress[i].RotateIfShould(src);
        }
    }

    /// <summary>
    /// Processes the input image directly and rotates it if necessary.
    /// </summary>
    /// <param name="src">The source image.</param>
    /// <exception cref="ArgumentException">Thrown if the source image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if the source image has a channel count other than 3 or 1.</exception>
    /// <remarks>No return value. The source images are modified directly.</remarks>
    public void Run(Mat src)
    {
        Ocr180DegreeClsResult res = ShouldRotate180(src);
        res.RotateIfShould(src);
    }

    private static Mat ResizePadding(Mat src, NCHW shape)
    {
        Size srcSize = src.Size();
        double whRatio = 1.0 * shape.Width / shape.Height;
        using Mat roi = 1.0 * srcSize.Width / srcSize.Height > whRatio ?
            src[0, srcSize.Height, 0, (int)Math.Floor(1.0 * srcSize.Height * whRatio)] :
            src.FastClone();

        double scaleRate = 1.0 * shape.Height / srcSize.Height;
        Mat resized = roi.Resize(new Size(Math.Floor(roi.Width * scaleRate), shape.Height));

        if (resized.Width < shape.Width)
        {
            Cv2.CopyMakeBorder(resized, resized, 0, 0, 0, shape.Width - resized.Width, BorderTypes.Constant, Scalar.Black);
        }
        return resized;
    }

    private static Mat Normalize(Mat src)
    {
        using Mat normalized = new();
        src.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
        Mat[] bgr = normalized.Split();
        float[] scales = new[] { 1 / 0.5f, 1 / 0.5f, 1 / 0.5f };
        float[] means = new[] { 0.5f, 0.5f, 0.5f };
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
