using Sdcb.OpenVINO.Natives;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Provides an abstract class for indexing into <see cref="Tensor"/> data.
/// </summary>
public abstract class TensorIndexer : IReadOnlyList<Tensor>
{
    internal readonly unsafe ov_infer_request* _req;

    internal unsafe TensorIndexer(ov_infer_request* req, int count)
    {
        _req = req;
        Count = count;
    }

    /// <summary>
    /// Gets the primary tensor.
    /// </summary>
    public abstract Tensor Primary { get; set; }

    /// <inheritdoc/>
    public int Count { get; }

    /// <summary>
    /// Indexes into the tensor data with the specified index.
    /// </summary>
    /// <param name="index">The index into the tensor data.</param>
    /// <returns>The tensor data indexed by the specified index.</returns>
    /// <exception cref="IndexOutOfRangeException"/>
    /// <exception cref="ObjectDisposedException"/>
    public abstract Tensor this[int index] { get; set; }

    internal unsafe void ThrowIfDisposed()
    {
        if (_req == null) throw new ObjectDisposedException($"{nameof(ov_infer_request)}* is null");
    }

    /// <inheritdoc/>
    public IEnumerator<Tensor> GetEnumerator()
    {
        for (int i = 0; i < Count; ++i)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

internal class InputTensorIndexer : TensorIndexer
{
    public unsafe InputTensorIndexer(ov_infer_request* inferRequest, int count) : base(inferRequest, count)
    {
    }

    public unsafe override Tensor this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));

            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_input_tensor_by_index(_req, index, &tensor));

            return new Tensor(tensor, owned: true);
        }
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_infer_request_set_input_tensor_by_index(_req, index, (ov_tensor*)value.DangerousGetHandle()));
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
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_infer_request_set_input_tensor(_req, (ov_tensor*)value.DangerousGetHandle()));
        }
    }
}

internal class OutputTensorIndexer : TensorIndexer
{
    public unsafe OutputTensorIndexer(ov_infer_request* inferRequest, int count) : base(inferRequest, count)
    {
    }

    public unsafe override Tensor this[int index]
    {
        get
        {
            if (index < 0 || index >= Count) throw new IndexOutOfRangeException(nameof(index));
            ThrowIfDisposed();

            ov_tensor* tensor;
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_output_tensor_by_index(_req, index, &tensor));

            return new Tensor(tensor, owned: true);
        }
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_infer_request_set_output_tensor_by_index(_req, index, (ov_tensor*)value.DangerousGetHandle()));
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
        set
        {
            ThrowIfDisposed();
            OpenVINOException.ThrowIfFailed(ov_infer_request_set_input_tensor(_req, (ov_tensor*)value.DangerousGetHandle()));
        }
    }
}
