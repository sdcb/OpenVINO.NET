using OpenCvSharp;
using System;

namespace Sdcb.OpenVINO.PaddleOCR;


/// <summary>
/// Represents the result of an OCR operation that determines whether an image needs to be rotated by 180 degrees.
/// </summary>
public readonly record struct Ocr180DegreeClsResult
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Ocr180DegreeClsResult"/> struct.
    /// </summary>
    /// <param name="shouldRotate180">Indicates whether the image should be rotated by 180 degrees.</param>
    /// <param name="confidence">The confidence level of the OCR's determination, expressed as a percentage between 0 and 1.</param>
    public Ocr180DegreeClsResult(bool shouldRotate180, float confidence)
    {
        ShouldRotate180 = shouldRotate180;
        Confidence = confidence;
    }

    /// <summary>
    /// Implicitly converts the <see cref="Ocr180DegreeClsResult"/> to a boolean value indicating whether the image should be rotated by 180 degrees.
    /// </summary>
    /// <param name="me">The <see cref="Ocr180DegreeClsResult"/> instance to convert.</param>
    /// <returns><c>true</c> if the image should be rotated by 180 degrees; otherwise, <c>false</c>.</returns>
    public static implicit operator bool(Ocr180DegreeClsResult me) => me.ShouldRotate180;

    internal Ocr180DegreeClsResult(ReadOnlySpan<float> softmax, double rotateThreshold)
    {
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

        ShouldRotate180 = label % 2 == 1 && score > rotateThreshold;
        Confidence = score;
    }

    /// <summary>
    /// Indicates whether the image should be rotated by 180 degrees.
    /// </summary>
    public bool ShouldRotate180 { get; }

    /// <summary>
    /// The confidence level of the OCR's determination, expressed as a percentage between 0 and 1.
    /// </summary>
    public float Confidence { get; }

    /// <summary>
    /// Rotates the input image by 180 degrees if the OCR's determination indicates that it should be rotated.
    /// </summary>
    /// <param name="src">The input image to be rotated.</param>
    public void RotateIfShould(Mat src)
    {
        if (ShouldRotate180)
        {
            Cv2.Rotate(src, src, RotateFlags.Rotate180);
        }
    }
}
