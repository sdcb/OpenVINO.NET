using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents the model loaded from an OpenVINO.
/// </summary>
public class Model : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Model"/> class.
    /// </summary>
    /// <param name="ptr">The pointer to the existing <see cref="ov_model"/>.</param>
    /// <param name="owned">If set to <c>true</c> the instance owns the handle.</param>
    public unsafe Model(ov_model* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
        Inputs = new InputPortIndexer((ov_model*)Handle);
        Outputs = new OutputPortIndexer((ov_model*)Handle);
    }

    /// <summary>
    /// Provides an indexer over the input nodes in the model.
    /// </summary>
    public unsafe PortIndexer Inputs { get; }

    /// <summary>
    /// Provides an indexer over the output nodes in the model.
    /// </summary>
    public unsafe PortIndexer Outputs { get; }

    /// <summary>
    /// Gets the friendly name of the model.
    /// </summary>
    /// <returns>The friendly name of the model as a string.</returns>
    public unsafe string FriendlyName
    {
        get
        {
            ThrowIfDisposed();

            byte* friendlyName;
            OpenVINOException.ThrowIfFailed(ov_model_get_friendly_name((ov_model*)Handle, &friendlyName));

            return StringUtils.UTF8PtrToString((IntPtr)friendlyName)!;
        }
    }

    /// <summary>
    /// Reshapes the model by changing shapes of named input/output tensors.
    /// </summary>
    /// <param name="shapes">An array of tuples containing the name and new shape for each tensor to be reshaped.</param>
    /// <exception cref="ObjectDisposedException" />
    public unsafe void ReshapeByTensorNames(params (string name, PartialShape shape)[] shapes)
    {
        ThrowIfDisposed();

        GCHandle[] gcs = shapes
            .Select(x => GCHandle.Alloc(Encoding.UTF8.GetBytes(x.name + '\0'), GCHandleType.Pinned))
            .ToArray();
        GCHandle[] dims = shapes
            .Select(x => GCHandle.Alloc(x.shape.Dimensions, GCHandleType.Pinned))
            .ToArray();
        try
        {
            ov_dimension[] ranks = shapes.Select(x => (ov_dimension)x.shape.Rank).ToArray();

            byte** names = stackalloc byte*[shapes.Length];
            ov_partial_shape* nshapes = stackalloc ov_partial_shape[shapes.Length];
            for (int i = 0; i < shapes.Length; ++i)
            {
                names[i] = (byte*)gcs[i].AddrOfPinnedObject();
                nshapes[i].dims = (ov_dimension*)dims[i].AddrOfPinnedObject();
                nshapes[i].rank = ranks[i];
            }

            OpenVINOException.ThrowIfFailed(ov_model_reshape((ov_model*)Handle, names, nshapes, shapes.Length));
        }
        finally
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                gcs[i].Free();
                dims[i].Free();
            }
        }
    }

    /// <summary>
    /// Reshapes the model by changing shapes of input/output tensors referred by their port indexes.
    /// </summary>
    /// <param name="shapes">A dictionary where each key-value pair represents an input/output tensor index and its new shape respectively.</param>
    /// <exception cref="ObjectDisposedException" />
    public unsafe void ReshapeByPortIndexes(Dictionary<int, PartialShape> shapes)
    {
        ThrowIfDisposed();

        nint[] portIndexes = shapes.Keys.Select(x => (nint)x).ToArray();
        GCHandle[] dims = shapes.Values
            .Select(x => GCHandle.Alloc(x.Dimensions, GCHandleType.Pinned))
            .ToArray();
        try
        {
            ov_dimension[] ranks = shapes.Values.Select(x => (ov_dimension)x.Rank).ToArray();

            byte** names = stackalloc byte*[shapes.Count];
            ov_partial_shape* nshapes = stackalloc ov_partial_shape[shapes.Count];
            for (int i = 0; i < shapes.Count; ++i)
            {
                nshapes[i].dims = (ov_dimension*)dims[i].AddrOfPinnedObject();
                nshapes[i].rank = ranks[i];
            }

            fixed (nint* portIndexPtr = portIndexes)
            {
                OpenVINOException.ThrowIfFailed(ov_model_reshape_by_port_indexes((ov_model*)Handle, portIndexPtr, nshapes, shapes.Count));
            }
        }
        finally
        {
            for (int i = 0; i < shapes.Count; i++)
            {
                dims[i].Free();
            }
        }
    }

    /// <summary>
    /// Reshapes a specific tensor of the model by changing its shape.
    /// </summary>
    /// <param name="tensorName">The name of the tensor to be reshaped.</param>
    /// <param name="shape">The new shape of the tensor.</param>
    /// <exception cref="ObjectDisposedException" />
    public unsafe void ReshapeByTensorName(string tensorName, PartialShape shape)
    {
        ThrowIfDisposed();

        fixed (byte* namePtr = Encoding.UTF8.GetBytes(tensorName + '\0'))
        {
            using NativePartialShapeWrapper l = shape.Lock();
            OpenVINOException.ThrowIfFailed(ov_model_reshape_input_by_name((ov_model*)Handle, namePtr, l.PartialShape));
        }
    }

    /// <summary>
    /// Reshapes the model by changing shapes of tensors referred by ports.
    /// </summary>
    /// <param name="shapes">An array of tuples containing the port and new shape for each tensor to be reshaped.</param>
    /// <exception cref="ObjectDisposedException" />
    public unsafe void ReshapeByPorts(params (IOPort port, PartialShape shape)[] shapes)
    {
        ThrowIfDisposed();

        ov_output_port** ports = stackalloc ov_output_port*[shapes.Length];
        GCHandle[] dims = new GCHandle[shapes.Length];
        for (int i = 0; i < shapes.Length; ++i)
        {
            ports[i] = (ov_output_port*)shapes[i].port.DangerousGetHandle();
            dims[i] = GCHandle.Alloc(shapes[i].shape.Dimensions, GCHandleType.Pinned);
        }
        try
        {
            ov_dimension[] ranks = shapes.Select(x => (ov_dimension)x.shape.Rank).ToArray();

            byte** names = stackalloc byte*[shapes.Length];
            ov_partial_shape* nshapes = stackalloc ov_partial_shape[shapes.Length];
            for (int i = 0; i < shapes.Length; ++i)
            {
                nshapes[i].dims = (ov_dimension*)dims[i].AddrOfPinnedObject();
                nshapes[i].rank = ranks[i];
            }

            OpenVINOException.ThrowIfFailed(ov_model_reshape_by_ports((ov_model*)Handle, ports, nshapes, shapes.Length));
        }
        finally
        {
            for (int i = 0; i < shapes.Length; i++)
            {
                dims[i].Free();
            }
        }
    }

    /// <summary>
    /// Reshapes the primary input of the model by changing its shape.
    /// </summary>
    /// <param name="shape">The new shape of the primary input.</param>
    /// <exception cref="ObjectDisposedException" />
    public unsafe void ReshapePrimaryInput(PartialShape shape)
    {
        ThrowIfDisposed();

        using NativePartialShapeWrapper l = shape.Lock();
        OpenVINOException.ThrowIfFailed(ov_model_reshape_single_input((ov_model*)Handle, l.PartialShape));
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_model_free((ov_model*)Handle);
    }
}
