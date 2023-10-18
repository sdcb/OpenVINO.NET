using System;

namespace Sdcb.OpenVINO;


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
    /// <param name="afterReadModel">The <see cref="Action&lt;Model&gt;"/> delegate that is invoked after the <see cref="Model"/> instance is created.</param>
    /// <param name="prePostProcessing">The <see cref="Action&lt;Model, PrePostProcessor&gt;"/> delegate that is invoked after the <see cref="PrePostProcessor"/> instance is created.</param>
    /// <param name="afterBuildModel">The <see cref="Action&lt;Model&gt;"/> delegate that is invoked after the <see cref="Model"/> instance is built from <see cref="PrePostProcessor"/>.</param>
    /// <param name="afterCompiledModel">The <see cref="Action&lt;CompiledModel&gt;"/> delegate that is invoked after the <see cref="CompiledModel"/> instance is created.</param>
    /// <returns>Returns an <see cref="InferRequest"/> instance.</returns>
    public virtual InferRequest CreateInferRequest(
        DeviceOptions? options = null,
        Action<Model>? afterReadModel = null,
        Action<Model, PrePostProcessor>? prePostProcessing = null,
        Action<Model>? afterBuildModel = null,
        Action<CompiledModel>? afterCompiledModel = null)
    {
        options ??= DefaultDeviceOptions;
        using OVCore core = options.CreateOVCore();

        using Model rawModel = CreateOVModel(core);
        afterReadModel?.Invoke(rawModel);
        AfterReadModel(rawModel);

        using PrePostProcessor ppp = rawModel.CreatePrePostProcessor();
        prePostProcessing?.Invoke(rawModel, ppp);
        PrePostProcessing(rawModel, ppp);

        using Model m = ppp.BuildModel();
        afterBuildModel?.Invoke(m);
        AfterBuildModel(m);

        using CompiledModel cm = core.CompileModel(m, options.DeviceName, options.Properties);
        afterCompiledModel?.Invoke(cm);
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
    /// Virtual method for preprocessing and postprocessing models created using OpenVINO.
    /// </summary>
    /// <remarks>
    /// This method is intended to be used to make any necessary changes to the model or the pre/postprocessing stages before compiling the model and creating an InferRequest instance.
    /// </remarks>
    /// <param name="model">The OpenVINO <see cref="Model"/> object.</param>
    /// <param name="prePostProcessor">The <see cref="PrePostProcessor"/> instance used for pre/postprocessing.</param>
    public virtual void PrePostProcessing(Model model, PrePostProcessor prePostProcessor) { }

    /// <summary>
    /// This method is called after a <see cref="Model"/> has been built by <see cref="PrePostProcessor"/>
    /// </summary>
    /// <param name="m">The <see cref="Model"/> object that has been built by <see cref="PrePostProcessor"/>.</param>
    public virtual void AfterBuildModel(Model m) { }

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
