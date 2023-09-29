using System;
using System.Collections.Generic;
using System.Text;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents a compiled model in OpenVINO.
/// </summary>
public class CompiledModel : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledModel"/> class.
    /// </summary>
    /// <param name="handle">The pointer to the compiled model.</param>
    /// <param name="owned">True if the object should be disposed of when it is no longer needed; otherwise, false.</param>
    public CompiledModel(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Gets the original <see cref="OpenVINO.Model"/> that this <see cref="CompiledModel"/> was created from.
    /// </summary>
    public unsafe Model Model
    {
        get
        {
            ov_model* model;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_get_runtime_model((ov_compiled_model*)Handle, &model));
            return new Model((IntPtr)model, owned: true);
        }
    }

    /// <summary>
    /// Frees the memory associated with the compiled model.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_compiled_model_free((ov_compiled_model*)Handle);
    }
}
