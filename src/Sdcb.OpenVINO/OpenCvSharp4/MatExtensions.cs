using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO.OpenCvSharp4;

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
}
