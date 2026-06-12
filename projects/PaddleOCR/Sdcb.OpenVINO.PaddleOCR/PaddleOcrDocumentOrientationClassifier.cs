using OpenCvSharp;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.Linq;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Classifies whole-document orientation as 0, 90, 180, or 270 degrees.
/// </summary>
public class PaddleOcrDocumentOrientationClassifier : IDisposable
{
    private static readonly int[] s_angles = { 0, 90, 180, 270 };

    private readonly CompiledModel _compiledModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrDocumentOrientationClassifier"/> class.
    /// </summary>
    public PaddleOcrDocumentOrientationClassifier(DocumentOrientationClassificationModel model, DeviceOptions? deviceOptions = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        _compiledModel = model.CreateCompiledModel(deviceOptions, prePostProcessing: (m, ppp) =>
        {
            using PreProcessInputInfo ppii = ppp.Inputs.Primary;
            ppii.TensorInfo.Layout = Layout.NHWC;
            ppii.ModelInfo.Layout = Layout.NCHW;
        });
    }

    /// <summary>
    /// Classifies document orientation.
    /// </summary>
    public PaddleOcrDocumentOrientationResult Run(Mat src)
    {
        return Run(new[] { src }).Single();
    }

    /// <summary>
    /// Classifies document orientation for a batch of images.
    /// </summary>
    public PaddleOcrDocumentOrientationResult[] Run(Mat[] srcs)
    {
        if (srcs == null) throw new ArgumentNullException(nameof(srcs));
        if (srcs.Length == 0) return Array.Empty<PaddleOcrDocumentOrientationResult>();

        foreach (Mat src in srcs)
        {
            if (src.Empty())
            {
                throw new ArgumentException("src size should not be 0, wrong input picture provided?");
            }

            if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
            {
                throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
            }
        }

        using Mat input = PrepareAndStackImages(srcs);
        using InferRequest request = _compiledModel.CreateInferRequest();
        using (Tensor tensor = input.StackedAsTensor(srcs.Length))
        {
            request.Inputs.Primary = tensor;
            request.Run();
        }

        using Tensor output = request.Outputs.Primary;
        ReadOnlySpan<float> data = output.GetData<float>();
        PaddleOcrDocumentOrientationResult[] results = new PaddleOcrDocumentOrientationResult[srcs.Length];
        for (int i = 0; i < srcs.Length; ++i)
        {
            ReadOnlySpan<float> scores = data.Slice(i * 4, 4);
            int maxIndex = 0;
            for (int j = 1; j < scores.Length; ++j)
            {
                if (scores[j] > scores[maxIndex])
                {
                    maxIndex = j;
                }
            }

            results[i] = new PaddleOcrDocumentOrientationResult(s_angles[maxIndex], scores[maxIndex], scores.ToArray());
        }

        return results;
    }

    /// <inheritdoc/>
    public void Dispose() => _compiledModel.Dispose();

    private static Mat PrepareAndStackImages(Mat[] srcs)
    {
        Mat[] normalizeds = null!;
        try
        {
            normalizeds = srcs.Select(PrepareInput).ToArray();
            return normalizeds.StackingVertically();
        }
        finally
        {
            if (normalizeds != null)
            {
                foreach (Mat normalized in normalizeds)
                {
                    normalized.Dispose();
                }
            }
        }
    }

    private static Mat PrepareInput(Mat src)
    {
        using Mat channel3 = src.Channels() switch
        {
            1 => src.CvtColor(ColorConversionCodes.GRAY2BGR),
            3 => src.FastClone(),
            var channelCount => throw new Exception($"Unexpect src channel: {channelCount}, allow: (1/3)")
        };
        using Mat resized = ResizeShort(channel3, 256);
        int x = (resized.Width - 224) / 2;
        int y = (resized.Height - 224) / 2;
        using Mat cropped = resized[y, y + 224, x, x + 224];
        using Mat rgb = cropped.CvtColor(ColorConversionCodes.BGR2RGB);

        Mat normalized = new();
        rgb.ConvertTo(normalized, MatType.CV_32FC3, 1.0 / 255);
        Mat[] channels = normalized.Split();
        normalized.Dispose();
        try
        {
            float[] means = { 0.485f, 0.456f, 0.406f };
            float[] stds = { 0.229f, 0.224f, 0.225f };
            for (int i = 0; i < channels.Length; ++i)
            {
                channels[i].ConvertTo(channels[i], MatType.CV_32FC1, 1.0 / stds[i], -means[i] / stds[i]);
            }

            Mat result = new();
            Cv2.Merge(channels, result);
            return result;
        }
        finally
        {
            foreach (Mat channel in channels)
            {
                channel.Dispose();
            }
        }
    }

    private static Mat ResizeShort(Mat src, int targetShort)
    {
        Size size = src.Size();
        double scale = targetShort / (double)Math.Min(size.Width, size.Height);
        return src.Resize(new Size((int)Math.Round(size.Width * scale), (int)Math.Round(size.Height * scale)));
    }
}
