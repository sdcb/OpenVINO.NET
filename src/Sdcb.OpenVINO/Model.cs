using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents the model loaded from an OpenVINO.
/// </summary>
public class Model : NativeResource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Model"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public Model(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <summary>
    /// Provides an indexer over the input nodes in the model.
    /// </summary>
    public unsafe IPortIndexer Inputs => new InputPortIndexer((ov_model*)Handle);

    /// <summary>
    /// Provides an indexer over the output nodes in the model.
    /// </summary>
    public unsafe IPortIndexer Output => new OutputPortIndexer((ov_model*)Handle);

    /// <inheritdoc/>
    protected unsafe override void ReleaseHandle(IntPtr handle)
    {
        ov_model_free((ov_model*)handle);
    }
}
