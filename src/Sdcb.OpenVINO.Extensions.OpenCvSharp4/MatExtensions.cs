using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO.Extensions.OpenCvSharp4;

/// <summary>
/// Provides extension methods for the <see cref="Mat"/> class.
/// </summary>
public static class MatExtensions
{
    /// <summary>
    /// Returns a weak reference to the specified <see cref="Mat"/> object.
    /// </summary>
    /// <param name="mat">The Mat object to create a weak reference to.</param>
    /// <returns>A weak reference to the specified Mat object.</returns>
    public static Mat WeakRef(this Mat mat)
    {
        Size size = mat.Size();
        MatType matType = mat.Type();
        return new Mat(size.Height, size.Width, matType, mat.Data);
    }

    /// <summary>
    /// Returns a <see cref="Span{T}"/> of <see cref="byte"/> that represents the underlying data of the <see cref="Mat"/> object.
    /// </summary>
    /// <param name="mat">The <see cref="Mat"/> object to create a <see cref="Span{T}"/> of <see cref="byte"/> from.</param>
    /// <returns>A <see cref="Span{T}"/> of <see cref="byte"/> that represents the underlying data of the <see cref="Mat"/> object.</returns>
    public static unsafe Span<byte> AsByteSpan(this Mat mat)
    {
        return new Span<byte>(mat.DataPointer, (int)((long)mat.DataEnd - (long)mat.DataStart));
    }

    /// <summary>
    /// Adds padding around a given image.
    /// </summary>
    /// <param name="src">Source image to apply padding to, represented as an OpenCvSharp4 Mat.</param>
    /// <param name="padSize">Optional parameter specifying the padding size. Default value is 32.</param>
    /// <param name="color">Optional parameter specifying the color of the padding. Default value is black.</param>
    /// <returns>
    /// Returns a new image with padding. If the source image size is already a multiple of the padding size, 
    /// a weak reference to the original image is returned to save memory. 
    /// Otherwise, the function creates a new image with the appropriate size, copies the source image into it,
    /// and fills in the additional space with the specified color.
    /// </returns>
    public static Mat Padding(Mat src, int padSize = 32, Scalar? color = default)
    {
        color ??= Scalar.Black;

        Size size = src.Size();
        Size newSize = new(
            32 * Math.Ceiling(1.0 * size.Width / padSize),
            32 * Math.Ceiling(1.0 * size.Height / padSize));
        if (newSize == size)
        {
            return src.WeakRef();
        }
        else
        {
            return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, color.Value);
        }
    }
}
