using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Nodes are the backbone of the graph of Value dataflow.
/// Every node has zero or more nodes as arguments and one value, which is either a tensor or a (possibly empty) tuple of values.
/// </summary>
public class IOPort : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IOPort"/> class.
    /// </summary>
    /// <param name="handle">The handle to the native resource.</param>
    /// <param name="isConst"><c>true</c> for <see cref="ov_output_const_port"/>, <c>false</c> for <see cref="ov_output_port"/>.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public IOPort(IntPtr handle, bool isConst, bool owned = true) : base(handle, owned)
    {
        IsConst = isConst;
    }

    internal unsafe IOPort(ov_output_port* port, bool owned = true) : this((IntPtr)port, isConst: false, owned)
    {
    }

    internal unsafe IOPort(ov_output_const_port* port, bool owned = true) : this((IntPtr)port, isConst: true, owned)
    {
    }

    /// <summary>
    /// <c>true</c> for <see cref="ov_output_const_port"/>, <c>false</c> for <see cref="ov_output_port"/>.
    /// </summary>
    public bool IsConst { get; }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        if (IsConst)
        {
            ov_output_const_port_free((ov_output_const_port*)Handle);
        }
        else
        {
            ov_output_port_free((ov_output_port*)Handle);
        }
    }
}
