using System;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents a class that contains information for pre-processing input models.
/// </summary>
public class InputModelInfo : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputModelInfo"/> class.
    /// </summary>
    /// <param name="handle">The handle.</param>
    /// <param name="owned">if set to <c>true</c> [owned].</param>
    public InputModelInfo(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InputModelInfo"/> class.
    /// </summary>
    /// <param name="ptr">The pointer.</param>
    /// <param name="owned">if set to <c>true</c> [owned].</param>
    public unsafe InputModelInfo(ov_preprocess_input_model_info* ptr, bool owned = true) : this((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// Sets the layout.
    /// </summary>
    public unsafe Layout Layout
    {
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_preprocess_input_model_info_set_layout((ov_preprocess_input_model_info*)Handle, (ov_layout*)value.DangerousGetHandle()));
        }
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_input_model_info_free((ov_preprocess_input_model_info*)Handle);
    }

    /// <summary>
    /// Returns a new instance of <see cref="InputModelInfo"/> with a reference to the current object.<br/>
    /// </summary>
    /// <returns>A new instance of <see cref="InputModelInfo"/> with a reference to the current object.</returns>
    public InputModelInfo WeakRef()
    {
        ThrowIfDisposed();

        return new InputModelInfo(Handle, owned: false);
    }
}
