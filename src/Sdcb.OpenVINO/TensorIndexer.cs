using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Provides an abstract class for indexing into <see cref="Tensor"/> data.
/// </summary>
public abstract class TensorIndexer
{
    internal readonly unsafe ov_infer_request* _req;

    internal unsafe TensorIndexer(ov_infer_request* req)
    {
        _req = req;
    }

    /// <summary>
    /// Gets the primary tensor.
    /// </summary>
    public abstract Tensor Primary { get; }

    /// <summary>
    /// Indexes into the tensor data with the specified index.
    /// </summary>
    /// <param name="index">The index into the tensor data.</param>
    /// <returns>The tensor data indexed by the specified index.</returns>
    public abstract Tensor this[int index] { get; }

    internal unsafe void ThrowIfDisposed()
    {
        if (_req == null) throw new ObjectDisposedException($"{nameof(ov_infer_request)}* is null");
    }
}

internal class InputTensorIndexer : TensorIndexer
{
    public unsafe InputTensorIndexer(ov_infer_request* inferRequest) : base(inferRequest)
    {
    }

    public unsafe override Tensor this[int index]
    {
        get
        {
            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_input_tensor_by_index(_req, index, &tensor));

            return new Tensor(tensor, owned: true);
        }
    }

    public unsafe override Tensor Primary
    {
        get
        {
            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_input_tensor(_req, &tensor));

            return new Tensor(tensor, owned: true);
        }
    }
}

internal class OutputTensorIndexer : TensorIndexer
{
    public unsafe OutputTensorIndexer(ov_infer_request* inferRequest) : base(inferRequest)
    {
    }

    public unsafe override Tensor this[int index]
    {
        get
        {
            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_output_tensor_by_index(_req, index, &tensor));

            return new Tensor(tensor, owned: true);
        }
    }

    public unsafe override Tensor Primary
    {
        get
        {
            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_output_tensor(_req, &tensor));

            return new Tensor(tensor, owned: true);
        }
    }
}
