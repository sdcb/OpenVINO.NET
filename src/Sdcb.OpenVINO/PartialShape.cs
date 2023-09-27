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
    /// Constructs a new instance of <see cref="PartialShape"/>.
    /// </summary>
    public unsafe PartialShape(Dimension rank, params Dimension[] dims) : base(owned: true)
    {
        fixed (ov_dimension* ptr = dims.Select(x => (ov_dimension)x).ToArray())
        fixed (ov_partial_shape* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_partial_shape_create_dynamic(rank, ptr, shapePtr));
        }
    }

    /// <summary>
    /// Constructs a new instance of <see cref="PartialShape"/>.
    /// </summary>
    public unsafe PartialShape(params Dimension[] dims) : base(owned: true)
    {
        fixed (ov_dimension* ptr = dims.Select(x => (ov_dimension)x).ToArray())
        fixed (ov_partial_shape* shapePtr = &_shape)
        {
            OpenVINOException.ThrowIfFailed(ov_partial_shape_create(dims.Length, ptr, shapePtr));
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
