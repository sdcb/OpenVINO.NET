using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// The class represents output tensor information during the pre-processing stage of the OpenVINO toolkit.
/// </summary>
public class PreProcessOutputInfo : CppPtrObject
{
    private readonly OutputTensorInfo _tensorInfo;

    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessOutputInfo"/> class with a handle and an optional ownership flag.
    /// </summary>
    /// <param name="handle">The handle of the pre-process output information.</param>
    /// <param name="owned">Whether the pre-process output information is owned or not.</param>
    public PreProcessOutputInfo(IntPtr handle, bool owned = true) : base(handle, owned)
    {
        _tensorInfo = GetTensorInfo();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessOutputInfo"/> class with a pre-process output tensor information pointer and an optional ownership flag.
    /// </summary>
    /// <param name="ptr">The pre-process output tensor information pointer.</param>
    /// <param name="owned">Whether the pre-process output tensor information is owned or not.</param>
    public unsafe PreProcessOutputInfo(ov_preprocess_output_info* ptr, bool owned = true) : this((IntPtr)ptr, owned)
    {
    }

    /// <summary>
    /// The output tensor information.
    /// </summary>
    public OutputTensorInfo TensorInfo => _tensorInfo.WeakRef();

    private unsafe OutputTensorInfo GetTensorInfo()
    {
        ThrowIfDisposed();

        ov_preprocess_output_tensor_info* tensorInfo;
        OpenVINOException.ThrowIfFailed(ov_preprocess_output_info_get_tensor_info((ov_preprocess_output_info*)Handle, &tensorInfo));
        return new OutputTensorInfo(tensorInfo, owned: false);
    }

    /// <inheritdoc />
    protected unsafe override void ReleaseCore()
    {
        ov_preprocess_output_info_free((ov_preprocess_output_info*)Handle);
    }
}
