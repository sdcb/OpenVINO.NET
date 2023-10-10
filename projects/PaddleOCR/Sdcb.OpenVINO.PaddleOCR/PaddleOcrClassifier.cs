using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Implements a PaddleOCR classifier using a PaddlePredictor.
/// </summary>
public class PaddleOcrClassifier : IDisposable
{
    private readonly InferRequest _p;

    /// <summary>
    /// Rotation threshold value used to determine if the image should be rotated.
    /// </summary>
    public double RotateThreshold { get; init; } = 0.75;

    /// <summary>
    /// The OcrShape used for the model.
    /// </summary>
    public NCHW Shape { get; init; } = ClassificationModel.DefaultShape;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrClassifier"/> class with a specified model and configuration.
    /// </summary>
    /// <param name="model">The <see cref="ClassificationModel"/> to use.</param>
    /// <param name="device">The device the inference request, pass null to using model's DefaultDevice.</param>
    public PaddleOcrClassifier(ClassificationModel model, DeviceOptions? device = null)
    {
        Shape = model.Shape;
        _p = model.CreateInferRequest(device);
    }

    /// <summary>
    /// Releases all resources used by the <see cref="PaddleOcrClassifier"/> object.
    /// </summary>
    public void Dispose() => _p.Dispose();

    /// <summary>
    /// Determines whether the image should be rotated by 180 degrees based on the threshold value.
    /// </summary>
    /// <param name="src">The source image.</param>
    /// <returns>True if the image should be rotated, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown if the source image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if the source image has a channel count other than 3 or 1.</exception>
    public bool ShouldRotate180(Mat src)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
        {
            throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
        }

        using Mat resized = ResizePadding(src, Shape);
        using Mat normalized = Normalize(resized);

        using (Tensor input = Tensor.FromArray(PaddleOcrDetector.ExtractMat(normalized), new Shape(1, 3, normalized.Rows, normalized.Cols)))
        {
            _p.Inputs.Primary = input;
        }

        _p.Run();

        using (Tensor output = _p.Outputs.Primary)
        {
            Span<float> softmax = output.GetData<float>();
            float score = 0;
            int label = 0;
            for (int i = 0; i < softmax.Length; ++i)
            {
                if (softmax[i] > score)
                {
                    score = softmax[i];
                    label = i;
                }
            }

            return label % 2 == 1 && score > RotateThreshold;
        }
    }

    /// <summary>
    /// Processes the input image, and returns the resulting image.
    /// </summary>
    /// <param name="src">The source image.</param>
    /// <returns>The resulting image.</returns>
    /// <exception cref="ArgumentException">Thrown if the source image size is 0.</exception>
    /// <exception cref="NotSupportedException">Thrown if the source image has a channel count other than 3 or 1.</exception>
    public Mat Run(Mat src)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
        {
            throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
        }

        if (ShouldRotate180(src))
        {
            Cv2.Rotate(src, src, RotateFlags.Rotate180);
            return src;
        }
        else
        {
            return src;
        }
    }

    private static Mat ResizePadding(Mat src, NCHW shape)
    {
        Size srcSize = src.Size();
        double whRatio = 1.0 * shape.Width / shape.Height;
        using Mat roi = 1.0 * srcSize.Width / srcSize.Height > whRatio ?
            src[0, srcSize.Height, 0, (int)Math.Floor(1.0 * srcSize.Height * whRatio)] :
            src.Clone();

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
        float[] scales = new[] { 2.0f, 2.0f, 2.0f };
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
