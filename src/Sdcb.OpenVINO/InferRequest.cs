using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// A class representing an inference request to the OpenVINO inference engine.
/// </summary>
public class InferRequest : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InferRequest"/> class with an pointer and an optional owned parameter.
    /// </summary>
    /// <param name="ptr">The pointer for the existing <see cref="ov_infer_request"/> object.</param>
    /// <param name="owned">A boolean indicating whether the underlying object is owned (default is true).</param>
    public unsafe InferRequest(ov_infer_request* ptr, bool owned = true) : base((IntPtr)ptr, owned)
    {
        Inputs = new InputTensorIndexer(ptr);
        Outputs = new OutputTensorIndexer(ptr);
    }

    /// <summary>
    /// Gets an indexer to access input tensors for the inference request.
    /// </summary>
    public TensorIndexer Inputs { get; }

    /// <summary>
    /// Gets an indexer to access output tensors for the inference request.
    /// </summary>
    public TensorIndexer Outputs { get; }

    /// <summary>
    /// Gets the tensor associated with the given IO port of the inference request.
    /// </summary>
    /// <param name="port">The input/output port for which to retrieve the tensor.</param>
    /// <returns>A newly created <see cref="Tensor"/> object that represents the tensor at the given input/output port.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the given IO port is null.</exception>
    public unsafe Tensor GetTensorByPort(IOPort port)
    {
        if (port == null) throw new ArgumentNullException(nameof(port));

        ov_tensor* tensor;

        if (port.IsConst)
        {
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_tensor_by_const_port((ov_infer_request*)Handle, (ov_output_const_port*)port.DangerousGetHandle(), &tensor));
        }
        else
        {
            OpenVINOException.ThrowIfFailed(ov_infer_request_get_tensor_by_port((ov_infer_request*)Handle, (ov_output_port*)port.DangerousGetHandle(), &tensor));
        }
        return new Tensor(tensor, owned: true);
    }

    /// <summary>
    /// Executes synchronous inference on the inference request.
    /// </summary>
    public unsafe void Infer()
    {
        OpenVINOException.ThrowIfFailed(ov_infer_request_infer((ov_infer_request*)Handle));
    }

    /// <summary>
    /// Executes asynchronous inference on the inference request.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous inference operation.</returns>
    public unsafe Task InferAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException($"{nameof(cancellationToken)} cancellation requested.");
        }

        TaskCompletionSource<int> tcs = new();

        GCHandle gch = GCHandle.Alloc(tcs);

        ov_callback_t cb = new()
        {
            args = GCHandle.ToIntPtr(gch).ToPointer(),
            callback_func = &InferCallback
        };

        OpenVINOException.ThrowIfFailed(ov_infer_request_set_callback((ov_infer_request*)Handle, &cb));

        cancellationToken.Register(() =>
        {
            if (!tcs.Task.IsCompleted)
            {
                tcs.TrySetCanceled();
                OpenVINOException.ThrowIfFailed(ov_infer_request_cancel((ov_infer_request*)Handle));
            }
        });

        OpenVINOException.ThrowIfFailed(ov_infer_request_start_async((ov_infer_request*)Handle));

        return tcs.Task;
    }

    private static unsafe void InferCallback(void* ptr)
    {
        GCHandle gch = GCHandle.FromIntPtr((IntPtr)ptr);

        try
        {
            TaskCompletionSource<int> tcs = (TaskCompletionSource<int>)gch.Target!;
            tcs.TrySetResult(0);
        }
        finally
        {
            gch.Free();
        }
    }

    /// <summary>
    /// Releases the underlying resources for the <see cref="InferRequest"/> object.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_infer_request_free((ov_infer_request*)Handle);
    }
}