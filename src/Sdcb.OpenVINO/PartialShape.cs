using Sdcb.OpenVINO.Natives;
using System;
using System.Linq;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents partial shape.
/// </summary>
public class PartialShape : CppObject
{
    ov_partial_shape _shape;

    /// <summary>
    /// Constructs a new instance of dynamic rank <see cref="PartialShape"/>.
    /// </summary>
    public unsafe PartialShape(Rank rank, params Dimension[] dims) : base(owned: true)
    {
        fixed (ov_dimension* dimsPtr = dims.Select(x => (ov_dimension)x).ToArray())
        fixed (ov_partial_shape* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_partial_shape_create_dynamic(rank, dimsPtr, shapePtr));
        }
    }

    /// <summary>
    /// Constructs a new instance of dynamic dimension <see cref="PartialShape"/>.
    /// </summary>
    public unsafe PartialShape(params Dimension[] dims) : base(owned: true)
    {
        fixed (ov_dimension* ptr = dims.Select(x => (ov_dimension)x).ToArray())
        fixed (ov_partial_shape* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_partial_shape_create(dims.Length, ptr, shapePtr));
        }
    }

    /// <summary>
    /// Constructs a new instance of static dimension <see cref="PartialShape"/>.
    /// </summary>
    public unsafe PartialShape(params long[] dims) : base(owned: true)
    {
        fixed (long* dimsPtr = dims)
        fixed (ov_partial_shape* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_partial_shape_create_static(dims.Length, dimsPtr, shapePtr));
        }
    }

    /// <inheritdoc/>
    public unsafe override bool Disposed => _shape.dims == null;

    /// <inheritdoc />
    protected unsafe override void ReleaseCore()
    {
        fixed (ov_partial_shape* ptr = &_shape)
        {
            ov_partial_shape_free(ptr);
            ptr->dims = null;
        }
    }

    /// <summary>
    /// Returns the string representation of the <see cref="PartialShape"/> object.
    /// </summary>
    /// <returns>The string representation of the <see cref="PartialShape"/> object.</returns>
    public unsafe override string ToString()
    {
        byte* strPtr = ov_partial_shape_to_string(_shape);
        return StringUtils.UTF8PtrToString((IntPtr)strPtr)!;
    }
}
