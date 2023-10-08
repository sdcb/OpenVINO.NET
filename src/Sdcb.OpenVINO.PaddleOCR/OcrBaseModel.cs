using Sdcb.OpenVINO.PaddleOCR.Models;
using System;
using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents an abstract base class for OCR models.
/// </summary>
/// <remarks>
/// The class serves as a base for optical character recognition(OCR) models.
/// </remarks>
public abstract class OcrBaseModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="OcrBaseModel"/> class.
    /// </summary>
    /// <param name="version">The version of model.</param>
    public OcrBaseModel(ModelVersion version)
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the OCR model.
    /// </summary>
    public ModelVersion Version { get; }
    
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
    /// Creates an instance of <see cref="InferRequest"/> class.
    /// </summary>
    /// <param name="options">used for the <see cref="InferRequest"/> instance creation.</param>
    /// <returns>Returns an <see cref="InferRequest"/> instance.</returns>
    public virtual InferRequest CreateInferRequest(DeviceOptions options)
    {
        using OVCore core = options.CreateCore();
        using Model m = CreateOVModel(core);
        AfterReadModel(m);
        using CompiledModel cm = core.CompileModel(m, options.DeviceName, options.Properties);
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

    /// <summary>
    /// Reads the inference model from a directory path and returns the <see cref="Model"/> instance.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to read the inference model from the directory path and return the pre-trained inference model wrapped in a <see cref="Model"/> object.
    /// </remarks>
    /// <param name="core">The OpenVINO Core used for reading the model.</param>
    /// <param name="directoryPath">The directory path where the inference model is located.</param>
    /// <returns>Returns a <see cref="Model"/> instance.</returns>
    public static Model ReadDirectoryInferenceModel(OVCore core, string directoryPath)
    {
        return core.ReadModel(Path.Combine(directoryPath, "inference.pdmodel"));
    }
}
