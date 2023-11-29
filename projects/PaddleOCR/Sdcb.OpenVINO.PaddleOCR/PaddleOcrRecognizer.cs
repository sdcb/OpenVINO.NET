using OpenCvSharp;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.Linq;
using System.Text;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Class for recognizing OCR using PaddleOCR models.
/// </summary>
public class PaddleOcrRecognizer : IDisposable
{
    private readonly CompiledModel _compiledModel;

    /// <summary>
    /// Recognization model being used for OCR.
    /// </summary>
    public RecognizationModel Model { get; init; }

    /// <summary>
    /// Gets the width of the static shape input for the OCR model. This property is particularly useful in cases 
    /// where the model input shape is not dynamic or it's desired to have a constant shape input. If `null`, 
    /// the shape is considered to be dynamic. The value is automatically rounded to the nearest upper multiple of 32 
    /// following the instance creation based on the provided `staticShapeWidth` value in the constructor.
    /// </summary>
    public int? StaticShapeWidth { get; }

    /// <summary>
    /// Gets a value indicating whether the input shape for the OCR model is dynamic or not.
    /// If this property returns `true`, it means that the model can accept input of various shapes.
    /// If it returns `false`, it indicates that the model requires a fixed input shape defined by the <see cref="StaticShapeWidth"/> property.
    /// </summary>
    public bool IsDynamic => StaticShapeWidth is null;

    /// <summary>
    /// The batch size used for inference. If not specified or zero, the batch size will be determined by minimum of 8 and the processor count.
    /// </summary>
    /// <remarks>
    /// The batch size determines the number of images that will be processed in parallel during inference. A larger batch size can lead to faster inference times, but may require more memory and may not fit on devices with limited memory.
    /// </remarks>
    public int BatchSize { get; set; }

    /// <summary>
    /// Constructor for creating a new instance of the <see cref="PaddleOcrRecognizer"/> class using a specified model and a callback configuration.
    /// </summary>
    /// <param name="model">The RecognizationModel object.</param>
    /// <param name="deviceOptions">The device of the inference request, pass null to using model's <see cref="BaseModel.DefaultDeviceOptions"/>.</param>
    /// <param name="staticShapeWidth">The width of the static shape for the OCR model. If provided, this value must be a positive integer. 
    /// The value will be rounded to the nearest upper multiple of 32. This parameter is useful for models that require a fixed shape input.
    /// Pass `null` if the model supports dynamic input shape.
    /// </param>
    public PaddleOcrRecognizer(RecognizationModel model, 
        DeviceOptions? deviceOptions = null, 
        int? staticShapeWidth = null)
    {
        Model = model;
        StaticShapeWidth = staticShapeWidth.HasValue ? (int)(32 * Math.Ceiling(1.0 * staticShapeWidth.Value / 32)) : null;

        _compiledModel = model.CreateCompiledModel(deviceOptions, prePostProcessing: (m, ppp) =>
        {
            using PreProcessInputInfo ppii = ppp.Inputs.Primary;
            ppii.TensorInfo.Layout = Layout.NHWC;
            ppii.ModelInfo.Layout = Layout.NCHW;
        }, afterBuildModel: m =>
        {
            if (StaticShapeWidth.HasValue)
            {
                m.ReshapePrimaryInput(new PartialShape(new Dimension(1, 128), 48, StaticShapeWidth.Value, 3));
            }
        });
    }

    /// <summary>
    /// Releases all resources used by the current instance of the <see cref="PaddleOcrRecognizer"/> class.
    /// </summary>
    public void Dispose() => _compiledModel.Dispose();

    /// <summary>
    /// Run OCR recognition on multiple images in batches.
    /// </summary>
    /// <param name="srcs">Array of images for OCR recognition.</param>
    /// <returns>Array of <see cref="PaddleOcrRecognizerResult"/> instances corresponding to OCR recognition results of the images.</returns>
    public PaddleOcrRecognizerResult[] Run(Mat[] srcs)
    {
        if (srcs.Length == 0)
        {
            return new PaddleOcrRecognizerResult[0];
        }

        int chooseBatchSize = BatchSize != 0 ? BatchSize : Math.Min(8, Environment.ProcessorCount);
        PaddleOcrRecognizerResult[] allResult = new PaddleOcrRecognizerResult[srcs.Length];

        return srcs
            .Select((x, i) => (mat: x, i, ratio: 1.0 * x.Width / x.Height))
            .OrderBy(x => x.ratio)
            .Chunk(chooseBatchSize)
            .Select(x => (result: BatchedRun(x.Select(x => x.mat).ToArray()), ids: x.Select(x => x.i).ToArray()))
            .SelectMany(x => x.result.Zip(x.ids, (result, i) => (result, i)))
            .OrderBy(x => x.i)
            .Select(x => x.result)
            .ToArray();
    }

    /// <summary>
    /// Run OCR recognition on a single image.
    /// </summary>
    /// <param name="src">Image for OCR recognition.</param>
    /// <returns><see cref="PaddleOcrRecognizerResult"/> instance corresponding to OCR recognition result of the image.</returns>
    public PaddleOcrRecognizerResult Run(Mat src) => BatchedRun(new[] { src }).Single();

    private unsafe PaddleOcrRecognizerResult[] BatchedRun(Mat[] srcs)
    {
        if (srcs.Length == 0)
        {
            return new PaddleOcrRecognizerResult[0];
        }

        for (int i = 0; i < srcs.Length; ++i)
        {
            Mat src = srcs[i];
            if (src.Empty())
            {
                throw new ArgumentException($"src[{i}] size should not be 0, wrong input picture provided?");
            }
        }

        int modelHeight = Model.Shape.Height;
        int maxWidth = StaticShapeWidth ?? (int)Math.Ceiling(srcs.Max(src =>
        {
            Size size = src.Size();
            double width = 1.0 * size.Width / size.Height * modelHeight;
            double padded = 32 * Math.Ceiling(1.0 * width / 32);
            return padded;
        }));

        using Mat final = PrepareAndStackImages(srcs, modelHeight, maxWidth);
        using InferRequest ir = _compiledModel.CreateInferRequest();
        using (Tensor input = final.StackedAsTensor(srcs.Length))
        {
            ir.Inputs.Primary = input;
            ir.Run();
        }
            
        using (Tensor output = ir.Outputs.Primary)
        {
            IntPtr dataPtr = output.DangerousGetDataPtr();
            int dataLength = (int)output.Size;

            Shape shape = output.Shape;

            int labelCount = shape[2];
            int charCount = shape[1];

            return Enumerable.Range(0, shape[0])
                .Select(i =>
                {
                    ReadOnlySpan<float> data = new((void*)dataPtr, dataLength); ;
                    StringBuilder sb = new();
                    int lastIndex = 0;
                    float score = 0;
                    for (int n = 0; n < charCount; ++n)
                    {
                        var start = (n + i * charCount) * labelCount;
                        var end = start + labelCount;
                        ReadOnlySpan<float> dataSlice = data.Slice(start, labelCount);
                        int maxIdx = MaxIndexOfSpan(dataSlice);
                        float maxVal = dataSlice[maxIdx];

                        if (maxIdx > 0 && (!(n > 0 && maxIdx == lastIndex)))
                        {
                            score += maxVal;
                            sb.Append(Model.GetLabelByIndex(maxIdx));
                        }
                        lastIndex = maxIdx;
                    }

                    return new PaddleOcrRecognizerResult(sb.ToString(), score / sb.Length);
                })
                .ToArray();
        }
    }

    static int MaxIndexOfSpan(ReadOnlySpan<float> data)
    {
        // 参数校验
        if (data.Length == 0) throw new ArgumentException("The provided tensor data should not be empty.");

        // 初始化最大值及其索引
        int maxIndex = 0;
        float maxValue = data[0];

        // 遍历跨度查找最大值及其索引
        for (int i = 1; i < data.Length; i++)
        {
            if (data[i] > maxValue)
            {
                maxValue = data[i];
                maxIndex = i;
            }
        }

        // 返回最大值及其索引
        return maxIndex;
    }

    private static unsafe Mat PrepareAndStackImages(Mat[] srcs, int modelHeight, int maxWidth)
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
                    return ResizePadding(channel3, modelHeight, maxWidth);
                })
                .ToArray();
            using Mat combined = normalizeds.StackingVertically();
            combined.ConvertTo(final, MatType.CV_32FC3, 2.0 / 255, -1.0);
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

    internal static Mat ResizePadding(Mat src, int modelHeight, int targetWidth)
    {
        // Calculate scaling factor
        double scale = Math.Min((double)modelHeight / src.Height, (double)targetWidth / src.Width);

        // Resize image
        Mat resized = new();
        Cv2.Resize(src, resized, new Size(), scale, scale);
        // Compute padding for height and width
        int padH = modelHeight - resized.Height;
        int padRight = targetWidth - resized.Width;

        if (padH > 0 || padRight > 0)
        {
            Mat result = new();
            int padTop = padH / 2;
            int padBottom = padH - padTop;
            Cv2.CopyMakeBorder(resized, result, padTop, padBottom, 0, padRight, BorderTypes.Constant, Scalar.Black);
            resized.Dispose();
            return result;
        }
        else
        {
            return resized;
        }
    }
}
