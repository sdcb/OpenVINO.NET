using OpenCvSharp;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents a region detected in an OCR result using Paddle OCR.
/// </summary>
public record struct PaddleOcrResultRegion(RotatedRect Rect, string Text, float Score);
