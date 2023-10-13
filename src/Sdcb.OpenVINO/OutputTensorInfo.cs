using System;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents an output tensor info.
/// </summary>
public class OutputTensorInfo : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTensorInfo"/> class.
    /// </summary>
    /// <param name="handle">Pointer to the output tensor info.</param>
    /// <param name="owned">Flag indicating whether the object should be disposed automatically.</param>
    public OutputTensorInfo(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OutputTensorInfo"/> class.
    /// </summary>
    /// <param name="ptr">Pointer to the input tensor info.</param>
    /// <param name="owned">Flag indicating whether the object should be disposed automatically.</param>
    public unsafe OutputTensorInfo(ov_preprocess_output_tensor_info* ptr, bool owned = true) : this((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Sets the element type of the output tensor.
    /// </summary>
    public unsafe ov_element_type_e ElementType
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_output_set_element_type((ov_preprocess_output_tensor_info*)Handle, value));
        }
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_output_tensor_info_free((ov_preprocess_output_tensor_info*)Handle);
    }

    /// <summary>
    /// Returns a weak reference to this instance.
    /// </summary>
    /// <returns> A new <see cref="InputTensorInfo"/> that represents a weak reference to this instance.</returns>
    public OutputTensorInfo WeakRef()
    {
        ThrowIfDisposed();

        return new OutputTensorInfo(Handle, owned: false);
    }
}
