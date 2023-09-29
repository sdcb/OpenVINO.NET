using System.Runtime.InteropServices;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

/// <summary>
/// A readonly ref struct for wrapping partial shapes.
/// </summary>
public readonly ref struct NativePartialShapeWrapper
{
    /// <summary>
    /// A handle used to pin the dimensions array in memory.
    /// </summary>
    private readonly GCHandle _arrayGCHandle;

    /// <summary>
    /// The partial shape being wrapped.
    /// </summary>
    public readonly ov_partial_shape PartialShape = default;

    /// <summary>
    /// Creates a new NativePartialShapeWrapper object with the specified shape.
    /// </summary>
    /// <param name="shape">The shape to wrap.</param>
    public unsafe NativePartialShapeWrapper(PartialShape shape)
    {
        _arrayGCHandle = GCHandle.Alloc(shape.Dimensions, GCHandleType.Pinned);
        PartialShape.rank = shape.Rank;
        PartialShape.dims = (ov_dimension*)_arrayGCHandle.AddrOfPinnedObject();
    }

    /// <summary>
    /// Frees the memory allocated for the dimensions array.
    /// </summary>
    public void Dispose()
    {
        _arrayGCHandle.Free();
    }
}
