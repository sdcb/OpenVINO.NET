using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents a tensor in OpenVINO.
/// </summary>
public class Tensor : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Tensor"/> class.
    /// </summary>
    /// <param name="handle">The handle to the tensor.</param>
    /// <param name="owned">Whether the handle is owned by this instance.</param>
    public Tensor(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Gets or sets the shape of the tensor.
    /// </summary>
    public unsafe Shape Shape
    {
        get
        {
            ThrowIfDisposed();

            ov_shape_t nshape;
            try
            {
                OpenVINOException.ThrowIfFailed(ov_tensor_get_shape((ov_tensor*)Handle, &nshape));
                return new Shape(nshape);
            }
            finally
            {
                ov_shape_free(&nshape);
            }
        }
        set
        {
            ThrowIfDisposed();

            using NativeShapeWrapper locked = value.Lock();
            OpenVINOException.ThrowIfFailed(ov_tensor_set_shape((ov_tensor*)Handle, locked.Shape));
        }
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_tensor_free((ov_tensor*)Handle);
    }
}
