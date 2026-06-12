using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.Linq;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents an OCR engine that uses PaddlePaddle models for object detection, classification, and recognition.
/// </summary>
public class PaddleOcrAll : IDisposable
{
    /// <summary>
    /// Gets the document orientation classifier used by this OCR engine, or null if no document classifier is used.
    /// </summary>
    public PaddleOcrDocumentOrientationClassifier? DocumentOrientationClassifier { get; }

    /// <summary>
    /// Gets the object detector used by this OCR engine.
    /// </summary>
    public PaddleOcrDetector Detector { get; }

    /// <summary>
    /// Gets the object classifier used by this OCR engine, or null if no classifier is used.
    /// </summary>
    public PaddleOcrClassifier? Classifier { get; }

    /// <summary>
    /// Gets the text recognizer used by this OCR engine.
    /// </summary>
    public PaddleOcrRecognizer Recognizer { get; }

    /// <summary>
    /// Gets or sets a value indicating whether to enable 180-degree classification.
    /// </summary>
    public bool Enable180Classification { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to enable full-document orientation classification.
    /// </summary>
    public bool EnableDocumentOrientationClassification { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to allow rotation detection.
    /// </summary>
    public bool AllowRotateDetection { get; set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and device configuration.
    /// </summary>
    /// <param name="model">The full OCR model containing detection, classification, and recognition models.</param>
    /// <param name="device">The device configuration for running det, cls and rec models.</param>
    public PaddleOcrAll(FullOcrModel model, DeviceOptions? device = null) : this(model, new PaddleOcrOptions(device ?? new DeviceOptions()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and options.
    /// </summary>
    /// <param name="model">The full OCR model containing detection, classification, and recognition models.</param>
    /// <param name="options">The options for running the OCR engine.</param>
    public PaddleOcrAll(FullOcrModel model, PaddleOcrOptions options)
    {
        if (model.DocumentOrientationModel != null)
        {
            DocumentOrientationClassifier = new PaddleOcrDocumentOrientationClassifier(model.DocumentOrientationModel, options.DocumentOrientationDeviceOptions);
        }
        Detector = new PaddleOcrDetector(model.DetectionModel, options.DetectionDeviceOptions, options.DetectionStaticSize);
        if (model.ClassificationModel != null)
        {
            Classifier = new PaddleOcrClassifier(model.ClassificationModel, options.ClassificationDeviceOptions);
        }
        Recognizer = new PaddleOcrRecognizer(model.RecognizationModel, options.RecognitionDeviceOptions, options.RecognitionStaticWidth);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified PaddlePaddle models and device configuration.
    /// </summary>
    /// <param name="detector">The object detector to use.</param>
    /// <param name="classifier">The object classifier to use, or null if no classifier is used.</param>
    /// <param name="recognizer">The text recognizer to use.</param>
    public PaddleOcrAll(PaddleOcrDetector detector, PaddleOcrClassifier? classifier, PaddleOcrRecognizer recognizer)
        : this(documentOrientationClassifier: null, detector, classifier, recognizer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrAll"/> class with the specified OCR pipeline components.
    /// </summary>
    /// <param name="documentOrientationClassifier">The document orientation classifier to use, or null if no document classifier is used.</param>
    /// <param name="detector">The object detector to use.</param>
    /// <param name="classifier">The text line orientation classifier to use, or null if no classifier is used.</param>
    /// <param name="recognizer">The text recognizer to use.</param>
    public PaddleOcrAll(
        PaddleOcrDocumentOrientationClassifier? documentOrientationClassifier,
        PaddleOcrDetector detector,
        PaddleOcrClassifier? classifier,
        PaddleOcrRecognizer recognizer)
    {
        DocumentOrientationClassifier = documentOrientationClassifier;
        Detector = detector;
        Classifier = classifier;
        Recognizer = recognizer;
    }

    /// <summary>
    /// Gets the cropped region of the source image specified by the given rectangle, clamping the rectangle coordinates to the image bounds.
    /// </summary>
    /// <param name="rect">The rectangle to crop.</param>
    /// <param name="size">The size of the source image.</param>
    /// <returns>The cropped rectangle.</returns>
    private static Rect GetCropedRect(Rect rect, Size size)
    {
        return Rect.FromLTRB(
            MathUtil.Clamp(rect.Left, 0, size.Width),
            MathUtil.Clamp(rect.Top, 0, size.Height),
            MathUtil.Clamp(rect.Right, 0, size.Width),
            MathUtil.Clamp(rect.Bottom, 0, size.Height));
    }

    /// <summary>
    /// Runs the OCR engine on the specified source image.
    /// </summary>
    /// <param name="src">The source image to run OCR on.</param>
    /// <returns>The OCR result.</returns>
    /// <exception cref="Exception">Thrown if 180-degree classification is enabled but no classifier is set.</exception>
    public PaddleOcrResult Run(Mat src)
    {
        if (Enable180Classification && Classifier == null)
        {
            throw new Exception($"Unable to do 180 degree Classification when classifier model is not set.");
        }

        using Mat? uprightSrc = CreateUprightImage(src, out int documentAngle);
        Mat ocrSrc = uprightSrc ?? src;

        RotatedRect[] rects = Detector.Run(ocrSrc);

        Mat[] mats = rects
            .Select(rect => AllowRotateDetection ? GetRotateCropImage(ocrSrc, rect) : ocrSrc[GetCropedRect(rect.BoundingRect(), ocrSrc.Size())])
            .ToArray();
        
        try
        {
            if (Enable180Classification)
            {
                Classifier!.Run(mats);
            }

            return new PaddleOcrResult(Recognizer.Run(mats)
                .Select((result, i) => new PaddleOcrResultRegion(MapRectToOriginal(rects[i], ocrSrc.Size(), documentAngle), result.Text, result.Score))
                .ToArray());
        }
        finally
        {
            foreach (Mat mat in mats)
            {
                mat.Dispose();
            }
        }
    }

    private Mat? CreateUprightImage(Mat src, out int documentAngle)
    {
        documentAngle = 0;
        if (!EnableDocumentOrientationClassification || DocumentOrientationClassifier == null)
        {
            return null;
        }

        PaddleOcrDocumentOrientationResult orientation = DocumentOrientationClassifier.Run(src);
        documentAngle = orientation.Angle;
        if (documentAngle == 0)
        {
            return null;
        }

        return orientation.RotateToUpright(src);
    }

    private static RotatedRect MapRectToOriginal(RotatedRect rect, Size uprightSize, int documentAngle)
    {
        if (documentAngle == 0)
        {
            return rect;
        }

        Point2f[] points = rect.Points()
            .Select(x => MapPointToOriginal(x, uprightSize, documentAngle))
            .ToArray();
        return Cv2.MinAreaRect(points);
    }

    private static Point2f MapPointToOriginal(Point2f point, Size uprightSize, int documentAngle)
    {
        return documentAngle switch
        {
            90 => new Point2f(uprightSize.Height - point.Y, point.X),
            180 => new Point2f(uprightSize.Width - point.X, uprightSize.Height - point.Y),
            270 => new Point2f(point.Y, uprightSize.Width - point.X),
            _ => point,
        };
    }

    /// <summary>
    /// Gets the cropped and rotated image specified by the given rectangle from the source image.
    /// </summary>
    /// <param name="src">The source image to crop and rotate.</param>
    /// <param name="rect">The rotated rectangle specifying the region to crop and rotate.</param>
    /// <returns>The cropped and rotated image.</returns>
    public static Mat GetRotateCropImage(Mat src, RotatedRect rect)
    {
        Point2f[] srcPoints = OrderPointsClockwise(rect.Points());

        int cropWidth = Math.Max(1, (int)Math.Round(Math.Max(
            GetDistance(srcPoints[0], srcPoints[1]),
            GetDistance(srcPoints[2], srcPoints[3]))));
        int cropHeight = Math.Max(1, (int)Math.Round(Math.Max(
            GetDistance(srcPoints[0], srcPoints[3]),
            GetDistance(srcPoints[1], srcPoints[2]))));

        Point2f[] dstPoints =
        {
            new(0, 0),
            new(cropWidth, 0),
            new(cropWidth, cropHeight),
            new(0, cropHeight),
        };

        using Mat matrix = Cv2.GetPerspectiveTransform(srcPoints, dstPoints);
        Mat dest = src.WarpPerspective(matrix, new Size(cropWidth, cropHeight), InterpolationFlags.Cubic, BorderTypes.Replicate);

        if (dest.Height * 1.0 / dest.Width >= 1.5)
        {
            Mat rotated = new();
            Cv2.Rotate(dest, rotated, RotateFlags.Rotate90Counterclockwise);
            dest.Dispose();
            return rotated;
        }

        return dest;
    }

    private static Point2f[] OrderPointsClockwise(Point2f[] points)
    {
        Point2f[] xSorted = points.OrderBy(p => p.X).ToArray();
        Point2f[] leftMost = xSorted.Take(2).OrderBy(p => p.Y).ToArray();
        Point2f[] rightMost = xSorted.Skip(2).ToArray();

        Point2f tl = leftMost[0];
        Point2f bl = leftMost[1];
        Point2f br = rightMost.OrderByDescending(p => GetDistance(tl, p)).First();
        Point2f tr = rightMost.Single(p => p != br);
        return new[] { tl, tr, br, bl };
    }

    private static double GetDistance(Point2f p1, Point2f p2)
    {
        double dx = p1.X - p2.X;
        double dy = p1.Y - p2.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    /// <summary>
    /// Releases the resources used by this OCR engine.
    /// </summary>
    public void Dispose()
    {
        DocumentOrientationClassifier?.Dispose();
        Detector.Dispose();
        Classifier?.Dispose();
        Recognizer.Dispose();
    }
}
