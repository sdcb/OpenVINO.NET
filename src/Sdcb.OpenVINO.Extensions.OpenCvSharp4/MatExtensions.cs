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
            padSize * Math.Ceiling(1.0 * size.Width / padSize),
            padSize * Math.Ceiling(1.0 * size.Height / padSize));
        if (newSize == size)
        {
            return src.WeakRef();
        }
        else
        {
            return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, color.Value);
        }
    }

    /// <summary>
    /// Stacks an array of Mats into a single Mat by placing them vertically.
    /// The resulted Mat is made by vertically stacking the input Mats, each having the same type and specified height and width.
    /// </summary>
    /// <param name="srcs">An array of Mats that are to be stacked. It's assumed that all provided Mats are having the same type.</param>
    /// <param name="height">The desired height for each Mat. All Matrices should have the same height.</param>
    /// <param name="width">The desired width for all Matrices. This width will also apply to resulted stacked Mat.</param>
    /// <returns>Returns a new Mat that is a vertical stack of all input Mats, each having the specified height and width.</returns>
    /// <exception cref="ArgumentException">Thrown when srcs is null or empty, or when any of the Mats in srcs does not match the specified type.</exception>
    /// <remarks>
    /// This method utilizes OpenCvSharp4 for operations on Mats. Pay attention to match correctly the type of Mats in the 'srcs' array.
    /// </remarks>
    public static Mat StackingVertically(this Mat[] srcs, int height, int width)
    {
        MatType matType = srcs[0].Type();
        Mat combinedMat = new(height * srcs.Length, width, matType, Scalar.Black);
        for (int i = 0; i < srcs.Length; i++)
        {
            Mat src = srcs[i];
            using Mat dest = combinedMat[i * height, (i + 1) * height, 0, src.Width];
            srcs[i].CopyTo(dest);
        }
        return combinedMat;
    }
}
