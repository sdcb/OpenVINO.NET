﻿using OpenCvSharp;
using System;

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
    /// <param name="height">The desired height for each Mat. All Matrices should have the same height.</param>
    /// <param name="width">The desired width for all Matrices. This width will also apply to resulted stacked Mat.</param>
    /// <returns>Returns a new Mat that is a vertical stack of all input Mats, each having the specified height and width.</returns>
    /// <exception cref="ArgumentException">Thrown when srcs is null or empty, or when any of the Mats in srcs does not match the specified type.</exception>
    /// <remarks>
    /// This method utilizes OpenCvSharp4 for operations on Mats. Pay attention to match correctly the type of Mats in the 'srcs' array.
    /// </remarks>
    [Obsolete("You can use another overloading method for better performance.")]
    public static Mat StackingVertically(this Mat[] srcs, int height, int width)
    {
        MatType matType = srcs[0].Type();
        Mat combinedMat = new(height * srcs.Length, width, matType, Scalar.Black);
        for (int i = 0; i < srcs.Length; i++)
        {
            Mat src = srcs[i];
            using Mat dest = combinedMat[i * height, (i + 1) * height, 0, src.Width];
            src.CopyTo(dest);
        }
        return combinedMat;
    }

    /// <summary>
    /// Stacks an array of Mats into a single Mat by placing them vertically.
    /// This version of StackingVertically is optimized for speed and requires the Mats to have the same width.
    /// </summary>
    /// <param name="srcs">An array of Mats that are to be stacked. All Mats must have the same width and type.</param>
    /// <returns>Returns a new Mat that is a vertical stack of all input Mats.</returns>
    /// <exception cref="OpenCVException">Thrown when the Mats within srcs have differing widths.</exception>
    /// <remarks>
    /// This method is an optimized version of vertical stacking where it is assumed that all Mats have the same width. 
    /// It straightly stacks the Mats in the given order, therefore the widths must be consistent to avoid issues.
    /// </remarks>
    public static Mat StackingVertically(this Mat[] srcs)
    {
        Mat combinedMat = new();
        Cv2.VConcat(srcs, combinedMat);
        return combinedMat;
    }
}
