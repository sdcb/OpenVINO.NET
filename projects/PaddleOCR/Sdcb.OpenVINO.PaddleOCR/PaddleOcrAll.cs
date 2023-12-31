﻿using OpenCvSharp;
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
    {
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

        RotatedRect[] rects = Detector.Run(src);

        Mat[] mats = rects
            .Select(rect => AllowRotateDetection ? GetRotateCropImage(src, rect) : src[GetCropedRect(rect.BoundingRect(), src.Size())])
            .ToArray();
        
        try
        {
            if (Enable180Classification)
            {
                Classifier!.Run(mats);
            }

            return new PaddleOcrResult(Recognizer.Run(mats)
                .Select((result, i) => new PaddleOcrResultRegion(rects[i], result.Text, result.Score))
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

    /// <summary>
    /// Gets the cropped and rotated image specified by the given rectangle from the source image.
    /// </summary>
    /// <param name="src">The source image to crop and rotate.</param>
    /// <param name="rect">The rotated rectangle specifying the region to crop and rotate.</param>
    /// <returns>The cropped and rotated image.</returns>
    public static Mat GetRotateCropImage(Mat src, RotatedRect rect)
    {
        bool wider = rect.Size.Width > rect.Size.Height;
        float angle = rect.Angle;
        Size srcSize = src.Size();
        Rect boundingRect = rect.BoundingRect();

        int expTop = Math.Max(0, 0 - boundingRect.Top);
        int expBottom = Math.Max(0, boundingRect.Bottom - srcSize.Height);
        int expLeft = Math.Max(0, 0 - boundingRect.Left);
        int expRight = Math.Max(0, boundingRect.Right - srcSize.Width);

        Rect rectToExp = boundingRect + new Point(expTop, expLeft);
        Rect roiRect = Rect.FromLTRB(
            boundingRect.Left + expLeft,
            boundingRect.Top + expTop,
            boundingRect.Right - expRight,
            boundingRect.Bottom - expBottom);
        using Mat boundingMat = src[roiRect];
        using Mat expanded = boundingMat.CopyMakeBorder(expTop, expBottom, expLeft, expRight, BorderTypes.Replicate);
        Point2f[] rp = rect.Points()
            .Select(v => new Point2f(v.X - rectToExp.X, v.Y - rectToExp.Y))
            .ToArray();

        Point2f[] srcPoints = (wider, angle) switch
        {
            (true, >= 0 and < 45) => new[] { rp[1], rp[2], rp[3], rp[0] },
            _ => new[] { rp[0], rp[3], rp[2], rp[1] }
        };

        var ptsDst0 = new Point2f(0, 0);
        var ptsDst1 = new Point2f(rect.Size.Width, 0);
        var ptsDst2 = new Point2f(rect.Size.Width, rect.Size.Height);
        var ptsDst3 = new Point2f(0, rect.Size.Height);

        using Mat matrix = Cv2.GetPerspectiveTransform(srcPoints, new[] { ptsDst0, ptsDst1, ptsDst2, ptsDst3 });

        Mat dest = expanded.WarpPerspective(matrix, new Size(rect.Size.Width, rect.Size.Height), InterpolationFlags.Nearest, BorderTypes.Replicate);

        if (!wider)
        {
            Cv2.Transpose(dest, dest);
        }
        else if (angle > 45)
        {
            Cv2.Flip(dest, dest, FlipMode.X);
        }
        return dest;
    }

    /// <summary>
    /// Releases the resources used by this OCR engine.
    /// </summary>
    public void Dispose()
    {
        Detector.Dispose();
        Classifier?.Dispose();
        Recognizer.Dispose();
    }
}
