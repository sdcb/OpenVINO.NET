using OpenCvSharp;
using System;
using System.Linq;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents the result of document orientation classification.
/// </summary>
public readonly record struct PaddleOcrDocumentOrientationResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrDocumentOrientationResult"/> struct.
    /// </summary>
    public PaddleOcrDocumentOrientationResult(int angle, float confidence, float[] scores)
    {
        Angle = angle;
        Confidence = confidence;
        Scores = scores;
    }

    /// <summary>
    /// Gets the detected clockwise orientation angle: 0, 90, 180, or 270.
    /// </summary>
    public int Angle { get; }

    /// <summary>
    /// Gets the confidence score for the detected angle.
    /// </summary>
    public float Confidence { get; }

    /// <summary>
    /// Gets the raw scores in 0, 90, 180, 270 order.
    /// </summary>
    public float[] Scores { get; }

    /// <summary>
    /// Returns a copy of the image rotated to upright orientation.
    /// </summary>
    public Mat RotateToUpright(Mat src)
    {
        if (src == null) throw new ArgumentNullException(nameof(src));

        Mat dest = new();
        switch (Angle)
        {
            case 90:
                Cv2.Rotate(src, dest, RotateFlags.Rotate90Counterclockwise);
                return dest;
            case 180:
                Cv2.Rotate(src, dest, RotateFlags.Rotate180);
                return dest;
            case 270:
                Cv2.Rotate(src, dest, RotateFlags.Rotate90Clockwise);
                return dest;
            default:
                src.CopyTo(dest);
                return dest;
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Angle={Angle}, Confidence={Confidence}, Scores=[{string.Join(", ", Scores.Select(x => x.ToString("0.000000")))}]";
    }
}
