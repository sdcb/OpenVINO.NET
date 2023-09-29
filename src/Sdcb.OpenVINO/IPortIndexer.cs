using Sdcb.OpenVINO.Natives;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sdcb.OpenVINO;

using static Sdcb.OpenVINO.Natives.NativeMethods;

/// <summary>
/// Represents an indexer for a collection of input <see cref="IOPort"/>s obtained from a specific OpenVINO model.
/// </summary>
public interface IPortIndexer : IReadOnlyList<IOPort>
{
    /// <summary>
    /// Gets the input IOPort with the specified name.
    /// </summary>
    /// <param name="name">The name of the input IOPort to retrieve.</param>
    /// <returns>The input IOPort with the specified name.</returns>
    public IOPort this[string name] { get; }

    /// <summary>
    /// Gets the primary input/output IOPort from the OpenVINO model.
    /// </summary>
    /// <remarks>
    /// The primary input IOPort is used as the model's input.
    /// </remarks>
    public IOPort Primary { get; }
}

internal unsafe class InputPortIndexer : IPortIndexer
{
    private readonly ov_model* model;

    public InputPortIndexer(ov_model* model)
    {
        this.model = model;
    }

    public IOPort Primary
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_input(model, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[int i]
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_input_by_index(model, i, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[string name]
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_model_const_input_by_name(model, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public int Count
    {
        get
        {
            nint size;
            OpenVINOException.ThrowIfFailed(ov_model_inputs_size(model, &size));
            return (int)size;
        }
    }

    public IEnumerator<IOPort> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal unsafe class OutputPortIndexer : IPortIndexer
{
    private readonly ov_model* model;

    public OutputPortIndexer(ov_model* model)
    {
        this.model = model;
    }

    public IOPort Primary
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_output(model, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[int i]
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_model_const_output_by_index(model, (nint)i, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[string name]
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_model_const_output_by_name(model, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public int Count
    {
        get
        {
            nint size;
            OpenVINOException.ThrowIfFailed(ov_model_outputs_size(model, &size));
            return (int)size;
        }
    }

    public IEnumerator<IOPort> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal unsafe class CompiledInputPortIndexer : IPortIndexer
{
    private readonly ov_compiled_model* model;

    public CompiledInputPortIndexer(ov_compiled_model* model)
    {
        this.model = model;
    }

    public IOPort Primary
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_input(model, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[int i]
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_input_by_index(model, i, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[string name]
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_compiled_model_input_by_name(model, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public int Count
    {
        get
        {
            nint size;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_inputs_size(model, &size));
            return (int)size;
        }
    }

    public IEnumerator<IOPort> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

internal unsafe class CompiledOutputPortIndexer : IPortIndexer
{
    private readonly ov_compiled_model* model;

    public CompiledOutputPortIndexer(ov_compiled_model* model)
    {
        this.model = model;
    }

    public IOPort Primary
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_output(model, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[int i]
    {
        get
        {
            ov_output_const_port* port;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_output_by_index(model, (nint)i, &port));
            return new IOPort(port);
        }
    }

    public IOPort this[string name]
    {
        get
        {
            byte[] bytes = Encoding.UTF8.GetBytes(name);
            fixed (byte* pBytes = bytes)
            {
                ov_output_const_port* port;
                OpenVINOException.ThrowIfFailed(ov_compiled_model_output_by_name(model, pBytes, &port));
                return new IOPort(port);
            }
        }
    }

    public int Count
    {
        get
        {
            nint size;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_outputs_size(model, &size));
            return (int)size;
        }
    }

    public IEnumerator<IOPort> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}