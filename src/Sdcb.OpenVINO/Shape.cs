using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents an OpenVINO tensor shape.
/// </summary>
public class Shape : CppObject, IList<long>
{
    ov_shape_t _shape;

    /// <summary>
    /// Creates a new <see cref="Shape"/> instance with 4 dimensions.
    /// </summary>
    /// <param name="d1">The first dimension.</param>
    /// <param name="d2">The second dimension.</param>
    /// <param name="d3">The third dimension.</param>
    /// <param name="d4">The fourth dimension.</param>
    public unsafe Shape(int d1, int d2, int d3, int d4) : base(owned: true)
    {
        long* dims = stackalloc long[4];
        dims[0] = d1;
        dims[1] = d2;
        dims[2] = d3;
        dims[3] = d4;
        fixed (ov_shape_t* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_shape_create(4, dims, shapePtr));
        }
        GC.AddMemoryPressure(sizeof(long) * 4);
    }

    /// <summary>
    /// Creates a new <see cref="Shape"/> instance from a <see cref="ReadOnlySpan{T}"/> of dimensions.
    /// </summary>
    /// <param name="dims">The span containing the dimensions of the shape.</param>
    public unsafe Shape(ReadOnlySpan<long> dims) : base(owned: true)
    {
        fixed (ov_shape_t* shapePtr = &_shape)
        fixed (long* dimsPtr = dims)
        {
            OpenVINOException.ThrowIfFailed(ov_shape_create(dims.Length, dimsPtr, shapePtr));
        }
        GC.AddMemoryPressure(sizeof(long) * dims.Length);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Shape"/> class with the given dimensions.
    /// </summary>
    /// <param name="dims">The dimensions of the <see cref="Shape"/> as an array of integers.</param>
    public unsafe Shape(params int[] dims) : this(dims.Select(x => (long)x).ToArray())
    {
    }

    /// <summary>
    /// Gets the number of dimensions in the shape tensor.
    /// </summary>
    public int Count
    {
        get
        {
            ThrowIfDisposed();

            return (int)_shape.rank;
        }
    }

    bool ICollection<long>.IsReadOnly => false;

    /// <inheritdoc/>
    public unsafe override bool Disposed => _shape.dims == null;

    /// <summary>
    /// Gets or sets the dimension at the specified index in the shape tensor.
    /// </summary>
    /// <param name="index">The index of the dimension to get or set.</param>
    /// <returns>The dimension at the specified index in the shape tensor.</returns>
    public unsafe long this[int index]
    {
        get
        {
            ThrowIfDisposed();
            if (index > _shape.rank)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _shape.dims[index];
        }
        set
        {
            ThrowIfDisposed();
            if (index > _shape.rank)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            _shape.dims[index] = value;
        }
    }

    /// <summary>
    /// Gets a <see cref="Span{T}"/> representation of the shape tensor.
    /// </summary>
    /// <returns>A <see cref="Span{T}"/> representation of the shape tensor.</returns>
    public unsafe Span<long> AsSpan()
    {
        ThrowIfDisposed();
        return new Span<long>(_shape.dims, (int)_shape.rank);
    }

    /// <summary>
    /// Gets a string representation of the <see cref="Shape"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="Shape"/>.</returns>
    public override string ToString()
    {
        return $"[{string.Join(",", this)}]";
    }

    /// <inheritdoc/>
    public unsafe int IndexOf(long item)
    {
        for (int i = 0; i < _shape.rank; ++i)
        {
            if (_shape.dims[i] == item)
            {
                return i;
            }
        }

        return -1;
    }

    void IList<long>.Insert(int index, long item) => throw new NotSupportedException();

    void IList<long>.RemoveAt(int index) => throw new NotSupportedException();

    void ICollection<long>.Add(long item) => throw new NotSupportedException();

    void ICollection<long>.Clear() => throw new NotSupportedException();

    /// <inheritdoc/>
    public unsafe bool Contains(long item)
    {
        for (long i = 0; i < _shape.rank; ++i)
        {
            if (_shape.dims[i] == item)
            {
                return true;
            }
        }
        return false;
    }

    /// <inheritdoc/>
    public void CopyTo(long[] array, int arrayIndex)
    {
        int r = Count;
        for (int i = 0; i < r; ++i)
        {
            array[arrayIndex + i] = this[i];
        }
    }

    bool ICollection<long>.Remove(long item) => throw new NotSupportedException();

    /// <inheritdoc/>
    public IEnumerator<long> GetEnumerator()
    {
        int r = Count;
        for (int i = 0; i < r; ++i)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        GC.RemoveMemoryPressure(sizeof(long) * Count);
        fixed (ov_shape_t* ptr = &_shape)
        {
            ov_shape_free(ptr); // will set _shape.dims to null
        }
    }
}
