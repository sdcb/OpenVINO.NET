using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Nodes are the backbone of the graph of Value dataflow.
/// Every node has zero or more nodes as arguments and one value, which is either a tensor or a (possibly empty) tuple of values.
/// </summary>
public class Node : NativeResource
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Node"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public Node(IntPtr handle, bool owned = true) : base(handle, owned)
    {
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseHandle(IntPtr handle)
    {
        ov_output_const_port_free((ov_output_const_port*)Handle);
    }
}
