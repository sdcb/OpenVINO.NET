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
    /// This method does not support Sub-matrix (ROI).
    /// </summary>
    /// <remarks>
    /// Note: This method has been marked as obsolete due to its inability to handle ROIs correctly. 
    /// When src is a ROI, the Mat returned from this function will be incorrect.
    /// </remarks>
    /// <param name="mat">The Mat object to create a weak reference to.</param>
    /// <returns>A weak reference to the specified Mat object.</returns>
    [Obsolete("WeakRef method does not correctly handle sub-matrices (ROI). Use the FastClone method instead.")]
    public static Mat WeakRef(this Mat mat)
    {
        Size size = mat.Size();
        MatType matType = mat.Type();
        return new Mat(size.Height, size.Width, matType, mat.Data);
    }

    /// <summary>
    /// Creates a clone of the specified <see cref="Mat"/> object.
    /// The clone shares the same memory space as the original Mat object.
    /// </summary>
    /// <remarks>
    /// Please note that write modifications or setting ROIs on either the new Mat or the original Mat may affect each other, 
    /// as they share the same memory space.
    /// </remarks>
    /// <param name="mat">The Mat object to clone.</param>
    /// <returns>A clone of the specified Mat object.</returns>
    public static Mat FastClone(this Mat mat)
    {
        Size size = mat.Size();
        return mat[0, size.Height, 0, size.Width];
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
            return src.FastClone();
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
    /// <param name="height">no need anymore,keep it just for compatibility</param>
    /// <param name="width">no need anymore,keep it just for compatibility</param>
    /// <returns>Returns a new Mat that is a vertical stack of all input Mats, each having the specified height and width.</returns>
    /// <exception cref="ArgumentException">Thrown when srcs is null or empty, or when any of the Mats in srcs does not match the specified type.</exception>
    /// <remarks>
    /// This method utilizes OpenCvSharp4 for operations on Mats. Pay attention to match correctly the type of Mats in the 'srcs' array.
    /// </remarks>
    public static Mat StackingVertically(this Mat[] srcs, int height, int width)
    {
        Mat combinedMat = new();
        Cv2.VConcat(this.Mats, combinedMat);
        return combinedMat;
    }
}
