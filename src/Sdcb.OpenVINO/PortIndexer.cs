using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents an indexer for a collection of input <see cref="IOPort"/>s obtained from a specific OpenVINO model.
/// </summary>
public abstract class PortIndexer : IReadOnlyList<IOPort>
{
    internal IntPtr Handle { get; }

    internal PortIndexer(IntPtr handle)
    {
        Handle = handle;
    }

    /// <summary>
    /// Gets the input IOPort with the specified name.
    /// </summary>
    /// <param name="name">The name of the input IOPort to retrieve.</param>
    /// <returns>The input IOPort with the specified name.</returns>
    public abstract IOPort this[string name] { get; }

    /// <inheritdoc/>
    public abstract IOPort this[int index] { get; }

    /// <summary>
    /// Gets the primary input/output IOPort from the OpenVINO model.
    /// </summary>
    /// <remarks>
    /// The primary input IOPort is used as the model's input.
    /// </remarks>
    public abstract IOPort Primary { get; }

    /// <inheritdoc/>
    public abstract int Count { get; }

    /// <inheritdoc/>
    public IEnumerator<IOPort> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void ThrowIfDisposed()
    {
        if (Handle == IntPtr.Zero) throw new ObjectDisposedException($"{nameof(Handle)} is null.");
    }
}

internal unsafe class InputPortIndexer : PortIndexer
{
    public unsafe InputPortIndexer(ov_model* model) : base((IntPtr)model)
    {
    }

    public override IOPort Primary
    {
        get
        {
            ThrowIfDisposed();

            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_input((ov_model*)Handle, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[int i]
    {
        get
        {
            ThrowIfDisposed();
            
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_input_by_index((ov_model*)Handle, i, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[string name]
    {
        get
        {
            ThrowIfDisposed();
            
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_model_const_input_by_name((ov_model*)Handle, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public override int Count
    {
        get
        {
            ThrowIfDisposed();
            
            nint size;
            OpenVINOException.ThrowIfFailed(ov_model_inputs_size((ov_model*)Handle, &size));
            return (int)size;
        }
    }    
}

internal unsafe class OutputPortIndexer : PortIndexer
{
    public OutputPortIndexer(ov_model* model) : base((IntPtr)model)
    {
    }

    public override IOPort Primary
    {
        get
        {
            ThrowIfDisposed();
            
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_output((ov_model*)Handle, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[int i]
    {
        get
        {
            ThrowIfDisposed();
            
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_output_by_index((ov_model*)Handle, (nint)i, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[string name]
    {
        get
        {
            ThrowIfDisposed();
            
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_model_const_output_by_name((ov_model*)Handle, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public override int Count
    {
        get
        {
            ThrowIfDisposed();

            nint size;
            OpenVINOException.ThrowIfFailed(ov_model_outputs_size((ov_model*)Handle, &size));
            return (int)size;
        }
    }
}

internal unsafe class CompiledInputPortIndexer : PortIndexer
{
    public CompiledInputPortIndexer(ov_compiled_model* model) : base((IntPtr)model)
    {
    }

    public override IOPort Primary
    {
        get
        {
            ThrowIfDisposed();

            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_input((ov_compiled_model*)Handle, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[int i]
    {
        get
        {
            ThrowIfDisposed();

            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_input_by_index((ov_compiled_model*)Handle, i, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[string name]
    {
        get
        {
            ThrowIfDisposed();

            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_compiled_model_input_by_name((ov_compiled_model*)Handle, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public override int Count
    {
        get
        {
            ThrowIfDisposed();

            nint size;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_inputs_size((ov_compiled_model*)Handle, &size));
            return (int)size;
        }
    }
}

internal unsafe class CompiledOutputPortIndexer : PortIndexer
{
    public CompiledOutputPortIndexer(ov_compiled_model* model) : base((IntPtr)model)
    {
    }

    public override IOPort Primary
    {
        get
        {
            ThrowIfDisposed();

            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_output((ov_compiled_model*)Handle, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[int i]
    {
        get
        {
            ThrowIfDisposed();

            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_output_by_index((ov_compiled_model*)Handle, (nint)i, &port));
            return new IOPort(port);
        }
    }

    public override IOPort this[string name]
    {
        get
        {
            ThrowIfDisposed();

            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_compiled_model_output_by_name((ov_compiled_model*)Handle, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public override int Count
    {
        get
        {
            ThrowIfDisposed();

            nint size;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_outputs_size((ov_compiled_model*)Handle, &size));
            return (int)size;
        }
    }
}