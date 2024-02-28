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
    /// <param name="inputTensorCount">The count of input tensors.</param>
    /// <param name="outputTensorCount">The count of the output tensors.</param>
    /// <param name="owned">A boolean indicating whether the underlying object is owned (default is true).</param>
    public unsafe InferRequest(ov_infer_request* ptr, int inputTensorCount, int outputTensorCount, bool owned = true) : base((IntPtr)ptr, owned)
    {
        Inputs = new InputTensorIndexer(ptr, inputTensorCount);
        Outputs = new OutputTensorIndexer(ptr, outputTensorCount);
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
    public unsafe void Run()
    {
        OpenVINOException.ThrowIfFailed(ov_infer_request_infer((ov_infer_request*)Handle));
    }

    /// <summary>
    /// <para>Executes asynchronous inference on the inference request.</para>
    /// NOTE: This function contains some unstable and hacky mechanisms that may potentially lead to crashes in your program.
    /// Use it with caution and make sure you understand the risks before invoking this function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous inference operation.</returns>
    [Obsolete("This method contains some unstable and hacky mechanisms that may potentially lead to crashes in your program. Use it with caution and make sure you understand the risks before invoking this function.")]
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            throw new TaskCanceledException($"{nameof(cancellationToken)} cancellation requested.");
        }

        TaskCompletionSource<int> tcs = new();

        SetCallback(ex => tcs.TrySetResult(0));

        cancellationToken.Register(() =>
        {
            if (!tcs.Task.IsCompleted)
            {
                tcs.TrySetCanceled();
                CancelAsyncRun();
            }
        });

        StartAsyncRun();

        await tcs.Task;
    }

    /// <summary>
    /// Sets a callback to the function and associates it with user data.
    /// </summary>
    /// <param name="callback"> The callback function to be used later. </param>
    /// <param name="userData"> Userdata which needs to be tied to callback. Default is null. </param>
    public unsafe void SetCallback(Action<IntPtr> callback, IntPtr userData = default)
    {
        ov_callback_t cb = new()
        {
            callback_func = (delegate*<void*, void>)Marshal.GetFunctionPointerForDelegate((void* ptr) =>
            {
                callback((IntPtr)ptr);
            }), 
            args = (void*)userData
        };

        OpenVINOException.ThrowIfFailed(ov_infer_request_set_callback((ov_infer_request*)Handle, &cb));
    }

    /// <summary>
    /// Starts an asynchronous inference request.
    /// </summary>
    public unsafe void StartAsyncRun()
    {
        OpenVINOException.ThrowIfFailed(ov_infer_request_start_async((ov_infer_request*)Handle));
    }

    /// <summary>
    /// Blocks the current thread and waits for the result of the inference to become available.
    /// </summary>
    public unsafe void WaitAsyncRun()
    {
        OpenVINOException.ThrowIfFailed(ov_infer_request_wait((ov_infer_request*)Handle));
    }

    /// <summary>
    /// Blocks the current thread and waits for a specified time for the inference result to become available.
    /// In version 2023.2 and later, the function may return <c>false</c> if the InferRequest has not been completed, 
    /// or <c>true</c> if the InferRequest has been completed successfully.
    /// In earlier versions, the completion status cannot be determined, hence the function always returns <c>true</c>.
    /// </summary>
    /// <param name="duration"> The TimeSpan object representing the duration to wait for the results. </param>
    /// <returns>
    /// For version 2023.2 and later, returns <c>false</c> if the InferRequest has not been completed, and <c>true</c> if it has.
    /// For earlier versions, always returns <c>true</c> as the completion status cannot be determined.
    /// </returns>
    public unsafe bool WaitAsyncRun(TimeSpan duration)
    {
        if (OpenVINOLibraryLoader.Is202302OrGreater())
        {
            ov_status_e res = ov_infer_request_wait_for((ov_infer_request*)Handle, (long)duration.TotalMilliseconds);
            if (res == ov_status_e.RESULT_NOT_READY) // 2023.3
            {
                return false;
            }
            else if (res == ov_status_e.UNEXPECTED) // 2023.2
            {
                return false;
            }
            else
            {
                OpenVINOException.ThrowIfFailed(res);
                return true;
            }
        }
        else
        {
            OpenVINOException.ThrowIfFailed(ov_infer_request_wait_for((ov_infer_request*)Handle, (long)duration.TotalMilliseconds));
            return true;
        }
    }

    /// <summary>
    /// Attempts to cancel an asynchronous inference request that has been initiated.
    /// </summary>
    public unsafe void CancelAsyncRun()
    {
        OpenVINOException.ThrowIfFailed(ov_infer_request_cancel((ov_infer_request*)Handle));
    }

    /// <summary>
    /// Releases the underlying resources for the <see cref="InferRequest"/> object.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_infer_request_free((ov_infer_request*)Handle);
    }
}