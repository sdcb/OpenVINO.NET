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

    /// <summary>
    /// Gets the name of the IOPort instance.
    /// </summary>
    public unsafe string Name
    {
        get
        {
            ThrowIfDisposed();

            byte* namePtr;
            OpenVINOException.ThrowIfFailed(ov_port_get_any_name((ov_output_const_port*)Handle, &namePtr));
            return StringUtils.UTF8PtrToString((IntPtr)namePtr)!;
        }
    }

    /// <summary>
    /// Gets the shape of the IOPort instance.
    /// </summary>
    public unsafe Shape Shape
    {
        get
        {
            ThrowIfDisposed();

            ov_shape_t shape;
            try
            {
                OpenVINOException.ThrowIfFailed(IsConst ?
                    ov_const_port_get_shape((ov_output_const_port*)Handle, &shape) :
                    ov_port_get_shape((ov_output_port*)Handle, &shape));
                return new Shape(shape);
            }
            finally
            {
                ov_shape_free(&shape);
            }
        }
    }

    /// <summary>
    /// Gets the partial shape of the IOPort instance.
    /// </summary>
    public unsafe PartialShape PartialShape
    {
        get
        {
            ThrowIfDisposed();

            ov_partial_shape shape = default;
            try
            {
                OpenVINOException.ThrowIfFailed(ov_port_get_partial_shape((ov_output_const_port*)Handle, &shape));
                return new PartialShape(shape);
            }
            finally
            {
                ov_partial_shape_free(&shape);
            }
        }
    }

    /// <summary>
    /// Gets the element type of the IOPort instance. 
    /// </summary>
    public unsafe ov_element_type_e ElementType
    {
        get
        {
            ThrowIfDisposed();

            ov_element_type_e elementType;
            OpenVINOException.ThrowIfFailed(ov_port_get_element_type((ov_output_const_port*)Handle, &elementType));

            return elementType;
        }
    }

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
