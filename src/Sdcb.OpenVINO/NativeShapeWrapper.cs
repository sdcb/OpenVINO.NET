using Sdcb.OpenVINO.Natives;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents an OpenVINO shape with pinned memory and allows manipulation of each element.
/// </summary>
/// <remarks>
/// This struct is used to access pinned memory for a <see cref="OpenVINO.Shape"/> instance.
/// </remarks>
public readonly ref struct NativeShapeWrapper
{
    private readonly GCHandle _arrayGCHandle;

    /// <summary>
    /// Gets the <see cref="ov_shape_t"/> structure representing the shape of the tensor.
    /// </summary>
    public readonly ov_shape_t Shape = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="NativeShapeWrapper"/> struct with the specified <see cref="OpenVINO.Shape"/> instance.
    /// </summary>
    /// <param name="shape">The <see cref="OpenVINO.Shape"/> instance to wrap.</param>
    public unsafe NativeShapeWrapper(Shape shape)
    {
        _arrayGCHandle = GCHandle.Alloc(shape.Dimensions, GCHandleType.Pinned);
        Shape.rank = shape.Count;
        Shape.dims = (long*)_arrayGCHandle.AddrOfPinnedObject();
    }

    /// <summary>
    /// Releases the associated <see cref="GCHandle"/> and releases any resources used by this instance.
    /// </summary>
    public void Dispose()
    {
        _arrayGCHandle.Free();
    }
}
