namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Contains extension methods to convert the input shape to NCHW format.
/// </summary>
public static class ShapeExtensions
{
    /// <summary>
    /// Converts the input shape to NCHW format.
    /// </summary>
    /// <param name="shape">Input shape to convert.</param>
    /// <returns>The converted shape in NCHW format.</returns>
    public static NCHW ToNCHW(this Shape shape) => NCHW.FromShape(shape);
}
