using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.Extensions.OpenCvSharp4;
using System;
using System.Linq;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// The PaddleOcrDetector class is responsible for detecting text regions in an image using PaddleOCR.
/// It implements IDisposable to manage memory resources.
/// </summary>
public class PaddleOcrDetector : IDisposable
{
    readonly InferRequestQueue _irQueue;

    /// <summary>
    /// Gets or sets the maximum size for resizing the input image.
    /// <para>Note: this property is invalid when <see cref="IsDynamicGraph"/> = <c>false</c></para>
    /// </summary>
    public int? MaxSize { get; set; } = 1536;

    /// <summary>Gets or sets the size for dilation during preprocessing.</summary>
    public int? DilatedSize { get; set; } = 2;

    /// <summary>Gets or sets the score threshold for filtering out possible text boxes.</summary>
    public float? BoxScoreThreahold { get; set; } = 0.7f;

    /// <summary>Gets or sets the threshold to binarize the text region.</summary>
    public float? BoxThreshold { get; set; } = 0.3f;

    /// <summary>Gets or sets the minimum size of the text boxes to be considered as valid.</summary>
    public int MinSize { get; set; } = 3;

    /// <summary>Gets or sets the ratio for enlarging text boxes during post-processing.</summary>
    public float UnclipRatio { get; set; } = 2.0f;

    /// <summary>
    /// Gets the static size of the input image for network infer.
    /// </summary>
    /// <remarks>
    /// If this property is null, network can work with image of any size and h/w ratio (dynamic).
    /// Otherwise, network works with fixed size image (static).
    /// </remarks>
    public Size? StaticShapeSize { get; } = null;

    /// <summary>
    /// Gets a value indicating whether the network uses dynamic graph.
    /// </summary>
    /// <value>
    ///   <c>true</c> if network uses dynamic graph; otherwise, <c>false</c>.
    /// </value>
    /// <remarks>
    /// A graph can be static or dynamic. Static graphs have a fixed structure determined before execution, while dynamic graphs have an undefined structure that emerges during execution. 
    /// </remarks>
    public bool IsDynamicGraph => !StaticShapeSize.HasValue;

    /// <summary>
    /// Initializes a new instance of the PaddleOcrDetector class with the provided DetectionModel and PaddleConfig.
    /// </summary>
    /// <param name="model">The DetectionModel to use.</param>
    /// <param name="options">The device and configure of the PaddleConfig, pass null to using model's DefaultDevice.</param>
    /// <param name="staticShapeSize">
    /// The static size of the input image for network infer, 
    /// <para>If this property is null, network can work with image of any size and h/w ratio (dynamic).</para>
    /// <para>Otherwise, network works with fixed size image (static).</para>
    /// </param>
    public PaddleOcrDetector(DetectionModel model, 
        DeviceOptions? options = null, 
        Size? staticShapeSize = null,
        InferRequestQueueOptions? irQueueOptions = null)
    {
        if (staticShapeSize != null)
        {
            StaticShapeSize = new(
                32 * Math.Ceiling(1.0 * staticShapeSize.Value.Width / 32),
                32 * Math.Ceiling(1.0 * staticShapeSize.Value.Height / 32));
        }

        CompiledModel cm = model.CreateCompiledModel(options, afterReadModel: m =>
        {
            if (model.Version != ModelVersion.V4)
            {
                m.ReshapePrimaryInput(new PartialShape(1, 3, Dimension.Dynamic, Dimension.Dynamic));
            }
        }, prePostProcessing: (m, ppp) =>
        {
            using PreProcessInputInfo ppii = ppp.Inputs.Primary;
            ppii.TensorInfo.Layout = Layout.NHWC;
            ppii.ModelInfo.Layout = Layout.NCHW;
        }, afterBuildModel: m =>
        {
            if (StaticShapeSize != null)
            {
                m.ReshapePrimaryInput(new PartialShape(1, StaticShapeSize.Value.Height, StaticShapeSize.Value.Width, 3));
            }
            else if (model.Version != ModelVersion.V4)
            {
                m.ReshapePrimaryInput(new PartialShape(1, Dimension.Dynamic, Dimension.Dynamic, 3));
            }
        });
        _irQueue = cm.CreateInferRequestQueue(irQueueOptions ?? InferRequestQueueOptions.Default);
    }

    /// <summary>
    /// Disposes the PaddlePredictor instance.
    /// </summary>
    public void Dispose()
    {
        _irQueue.Dispose();
    }

    /// <summary>
    /// Draws detected rectangles on the input image.
    /// </summary>
    /// <param name="src">Input image.</param>
    /// <param name="rects">Array of detected rectangles.</param>
    /// <param name="color">Color of the rectangles.</param>
    /// <param name="thickness">Thickness of the rectangle lines.</param>
    /// <returns>A new image with the detected rectangles drawn on it.</returns>
    public static Mat Visualize(Mat src, RotatedRect[] rects, Scalar color, int thickness)
    {
        Mat clone = src.Clone();
        clone.DrawContours(rects.Select(x => x.Points().Select(x => (Point)x)), -1, color, thickness);
        return clone;
    }

    /// <summary>
    /// Runs the text box detection process on the input image.
    /// </summary>
    /// <param name="src">Input image.</param>
    /// <returns>An array of detected rotated rectangles representing text boxes.</returns>
    /// <exception cref="ArgumentException">Thrown when input image is empty.</exception>
    /// <exception cref="NotSupportedException">Thrown when input image channels are not 3 or 1.</exception>
    /// <exception cref="Exception">Thrown when PaddlePredictor run fails.</exception>
    public RotatedRect[] Run(Mat src)
    {
        using Mat pred = RunRaw(src, out Size resizedSize);
        using Mat cbuf = new();
        {
            using Mat roi = pred[0, resizedSize.Height, 0, resizedSize.Width];
            roi.ConvertTo(cbuf, MatType.CV_8UC1, 255);
        }
        using Mat dilated = new();
        {
            using Mat binary = BoxThreshold != null ?
                cbuf.Threshold((int)(BoxThreshold * 255), 255, ThresholdTypes.Binary) :
                cbuf;

            if (DilatedSize != null)
            {
                using Mat ones = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(DilatedSize.Value, DilatedSize.Value));
                Cv2.Dilate(binary, dilated, ones);
            }
            else
            {
                Cv2.CopyTo(binary, dilated);
            }
        }

        Point[][] contours = dilated.FindContoursAsArray(RetrievalModes.List, ContourApproximationModes.ApproxSimple);
        Size size = src.Size();
        double scaleRate = 1.0 * src.Width / resizedSize.Width;

        RotatedRect[] rects = contours
            .Where(x => BoxScoreThreahold == null || GetScore(x, pred) > BoxScoreThreahold)
            .Select(x => Cv2.MinAreaRect(x))
            .Where(x => x.Size.Width > MinSize && x.Size.Height > MinSize)
            .Select(rect =>
            {
                float minEdge = Math.Min(rect.Size.Width, rect.Size.Height);
                Size2f newSize = new(
                    (rect.Size.Width + UnclipRatio * minEdge) * scaleRate,
                    (rect.Size.Height + UnclipRatio * minEdge) * scaleRate);
                RotatedRect largerRect = new(rect.Center * scaleRate, newSize, rect.Angle);
                return largerRect;
            })
            .OrderBy(v => v.Center.Y)
            .ThenBy(v => v.Center.X)
            .ToArray();
        //{
        //	using Mat demo = dilated.CvtColor(ColorConversionCodes.GRAY2RGB);
        //	demo.DrawContours(contours, -1, Scalar.Red);
        //	Image(demo).Dump();
        //}
        return rects;
    }

    /// <summary>
    /// Runs detection on the provided input image and returns the detected image as a <see cref="MatType.CV_32FC1"/> <see cref="Mat"/> object.
    /// </summary>
    /// <param name="src">The input image to run detection model on.</param>
    /// <param name="resizedSize">The returned image actuall size without padding.</param>
    /// <returns>the detected image as a <see cref="MatType.CV_32FC1"/> <see cref="Mat"/> object.</returns>
    public unsafe Mat RunRaw(Mat src, out Size resizedSize)
    {
        if (src.Empty())
        {
            throw new ArgumentException("src size should not be 0, wrong input picture provided?");
        }

        if (!(src.Channels() switch { 3 or 1 => true, _ => false }))
        {
            throw new NotSupportedException($"{nameof(src)} channel must be 3 or 1, provided {src.Channels()}.");
        }

        Mat padded = null!;
        if (IsDynamicGraph)
        {
            using Mat resized = MatResize(src, MaxSize);
            resizedSize = new Size(resized.Width, resized.Height);
            padded = MatPadding32(resized);
        }
        else
        {
            using Mat resized = MatResize(src, StaticShapeSize!.Value);
            resizedSize = new Size(resized.Width, resized.Height);
            padded = resized.CopyMakeBorder(0, StaticShapeSize.Value.Height - resizedSize.Height, 0, StaticShapeSize.Value.Width - resizedSize.Width, BorderTypes.Constant, Scalar.Black);
        }

        Mat normalized;
        using (Mat _ = padded)
        {
            normalized = Normalize(padded);
        }

        using InferRequestWrapper irWrapper = _irQueue.Using();
        using (Mat _ = normalized)
        using (Tensor input = normalized.AsTensor())
        {
            irWrapper.InferRequest.Inputs.Primary = input;
            irWrapper.InferRequest.Run();
        }

        using (Tensor output = irWrapper.InferRequest.Outputs[0])
        {
            Span<float> data = output.GetData<float>();
            NCHW shape = output.Shape.ToNCHW();

            return new Mat(shape.Height, shape.Width, MatType.CV_32FC1, data.ToArray());
        }
    }

    private static float GetScore(Point[] contour, Mat pred)
    {
        int width = pred.Width;
        int height = pred.Height;
        int[] boxX = contour.Select(v => v.X).ToArray();
        int[] boxY = contour.Select(v => v.Y).ToArray();

        int xmin = MathUtil.Clamp(boxX.Min(), 0, width - 1);
        int xmax = MathUtil.Clamp(boxX.Max(), 0, width - 1);
        int ymin = MathUtil.Clamp(boxY.Min(), 0, height - 1);
        int ymax = MathUtil.Clamp(boxY.Max(), 0, height - 1);

        Point[] rootPoints = contour
            .Select(v => new Point(v.X - xmin, v.Y - ymin))
            .ToArray();
        using Mat mask = new(ymax - ymin + 1, xmax - xmin + 1, MatType.CV_8UC1, Scalar.Black);
        mask.FillPoly(new[] { rootPoints }, new Scalar(1));

        using Mat croppedMat = pred[ymin, ymax + 1, xmin, xmax + 1];
        float score = (float)croppedMat.Mean(mask).Val0;

        return score;
    }

    private static Mat MatResize(Mat src, int? maxSize)
    {
        if (maxSize == null) return src.WeakRef();

        Size size = src.Size();
        int longEdge = Math.Max(size.Width, size.Height);
        double scaleRate = 1.0 * maxSize.Value / longEdge;

        return scaleRate < 1.0 ?
            src.Resize(Size.Zero, scaleRate, scaleRate) :
            src.WeakRef();
    }

    private static Mat MatResize(Mat src, Size maxSize)
    {
        Size srcSize = src.Size();
        if (srcSize == maxSize)
        {
            return src.WeakRef();
        }

        double scale = Math.Min(maxSize.Width / (double)srcSize.Width, maxSize.Height / (double)srcSize.Height);

        // Ensure the scale is never more than 1 (i.e., the image is never magnified)
        scale = Math.Min(scale, 1.0);

        if (scale == 1)
        {
            return src.WeakRef();
        }
        else
        {
            // New size
            Size newSize = new((int)(scale * srcSize.Width), (int)(scale * srcSize.Height));

            // Set the resized image
            Mat resizedMat = new();

            Cv2.Resize(src, resizedMat, newSize);

            return resizedMat;
        }
    }

    private static Mat MatPadding32(Mat src)
    {
        Size size = src.Size();
        Size newSize = new(
            32 * Math.Ceiling(1.0 * size.Width / 32),
            32 * Math.Ceiling(1.0 * size.Height / 32));
        return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, Scalar.Black);
    }

    private static Mat Normalize(Mat src)
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
