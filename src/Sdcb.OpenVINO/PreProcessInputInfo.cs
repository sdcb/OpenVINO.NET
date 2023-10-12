using System;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents pre-processing information for an input.
/// </summary>
public class PreProcessInputInfo : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessInputInfo"/> class.
    /// </summary>
    /// <param name="handle">The handle to the pre-processing information.</param>
    /// <param name="owned">A boolean flag indicating whether the current instance owns the handle.</param>
    public PreProcessInputInfo(IntPtr handle, bool owned = true) : base(handle, owned)
    {
        _steps = GetSteps();
        _modelInfo = GetModelInfo();
        _tensorInfo = GetTensorInfo();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreProcessInputInfo"/> class.
    /// </summary>
    /// <param name="ptr">A pointer to the pre-processing information.</param>
    /// <param name="owned">A boolean flag indicating whether the current instance owns the pointer.</param>
    public unsafe PreProcessInputInfo(ov_preprocess_input_info* ptr, bool owned = true) : this((IntPtr)ptr, owned)
    {
    }

    private readonly PreProcessSteps _steps;

    /// <summary>
    /// Gets the pre-processing steps.
    /// </summary>
    public PreProcessSteps Steps => _steps.WeakRef();

    private unsafe PreProcessSteps GetSteps()
    {
        ov_preprocess_preprocess_steps* steps;
        OpenVINOException.ThrowIfFailed(ov_preprocess_input_info_get_preprocess_steps((ov_preprocess_input_info*)Handle, &steps));
        return new PreProcessSteps(steps, owned: true);
    }

    private readonly InputModelInfo _modelInfo;

    /// <summary>
    /// Gets the model information for pre-processing.
    /// </summary>
    public InputModelInfo ModelInfo => _modelInfo.WeakRef();

    private unsafe InputModelInfo GetModelInfo()
    {
        ov_preprocess_input_model_info* model;
        OpenVINOException.ThrowIfFailed(ov_preprocess_input_info_get_model_info((ov_preprocess_input_info*)Handle, &model));
        return new InputModelInfo(model, owned: true);
    }

    private readonly InputTensorInfo _tensorInfo;

    /// <summary>
    /// Gets the tensor information for pre-processing.
    /// </summary>
    public InputTensorInfo TensorInfo => _tensorInfo.WeakRef();

    private unsafe InputTensorInfo GetTensorInfo()
    {
        ov_preprocess_input_tensor_info* tensor;
        OpenVINOException.ThrowIfFailed(ov_preprocess_input_info_get_tensor_info((ov_preprocess_input_info*)Handle, &tensor));
        return new InputTensorInfo(tensor, owned: true);
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        _steps.Dispose();
        _modelInfo.Dispose();
        _tensorInfo.Dispose();
        ov_preprocess_input_info_free((ov_preprocess_input_info*)Handle);
    }
}
