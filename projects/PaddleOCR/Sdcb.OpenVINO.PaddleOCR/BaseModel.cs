using System;
using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents an abstract base class for all models.
/// </summary>
public abstract class BaseModel
{
    /// <summary>
    /// Creates an OpenVINO model using the specified <see cref="OVCore"/>.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to be able to create an OpenVINO <see cref="Model"/> from the core that is passed to it.
    /// </remarks>
    /// <param name="core">The OpenVINO core.</param>
    /// <returns>Returns a <see cref="Model"/> instance.</returns>
    public abstract Model CreateOVModel(OVCore core);

    /// <summary>
    /// Creates an instance of <see cref="InferRequest"/> class using the specified <see cref="DeviceOptions"/>.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to be able to create an instance of <see cref="InferRequest"/> class from the <see cref="DeviceOptions"/> that are passed to it.
    /// It utilizes the <see cref="CreateOVModel(OVCore)"/> method to create an <see cref="Model"/> instance from the specified <see cref="OVCore"/>.
    /// After the model is created, it invokes the <see cref="AfterReadModel(Model)"/> method to make any necessary adjustments to the model.
    /// Then it compiles the model using the specified device and properties using <see cref="OVCore.CompileModel(Model, string, System.Collections.Generic.Dictionary{string, string}?)"/> method. After the model is compiled, it invokes <see cref="AfterCompiledModel(Model, CompiledModel)"/> to make any necessary adjustment to the compile model, and then it returns the created <see cref="InferRequest"/> instance.
    /// </remarks>
    /// <param name="options">The <see cref="DeviceOptions"/> instance to be used for creating the <see cref="InferRequest"/> instance.</param>
    /// <param name="readModelCallback">The <see cref="Action&lt;Model&gt;"/> delegate that is invoked after the <see cref="Model"/> instance is created.</param>
    /// <param name="compiledModelCallback">The <see cref="Action&lt;CompiledModel&gt;"/> delegate that is invoked after the <see cref="CompiledModel"/> instance is created.</param>
    /// <returns>Returns an <see cref="InferRequest"/> instance.</returns>
    public virtual InferRequest CreateInferRequest(
        DeviceOptions? options = null, 
        Action<Model>? readModelCallback = null, 
        Action<CompiledModel>? compiledModelCallback = null)
    {
        options ??= DefaultDeviceOptions;
        using OVCore core = options.CreateCore();

        using Model m = CreateOVModel(core);
        readModelCallback?.Invoke(m);
        AfterReadModel(m);

        using CompiledModel cm = core.CompileModel(m, options.DeviceName, options.Properties);
        compiledModelCallback?.Invoke(cm);
        AfterCompiledModel(m, cm);

        return cm.CreateInferRequest();
    }

    /// <summary>
    /// Call after reading the model.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to be able to access/adjust more information about the model such as statistics or details.</remarks>
    /// <param name="model">The OpenVINO <see cref="Model"/> object.</param>
    public virtual void AfterReadModel(Model model) { }

    /// <summary>
    /// Call after compiled model.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to be able to access/adjust more information about the compiled model such as statistics or details. It is utilized by subclasses if they need to store or access the compiled model or if necessary changes after compilation of the model need to be made.
    /// </remarks>
    /// <param name="model">The OpenVINO <see cref="Model"/> object.</param>
    /// <param name="compiledModel">The compiled OpenVINO <see cref="Model"/> object.</param>
    public virtual void AfterCompiledModel(Model model, CompiledModel compiledModel) { }

    /// <summary>
    /// Gets the default device options for the OCR model.
    /// </summary>
    /// <remarks>
    /// The device options include the device name and properties to be used during inference.
    /// </remarks>
    public DeviceOptions DefaultDeviceOptions { get; init; } = new DeviceOptions();
}
