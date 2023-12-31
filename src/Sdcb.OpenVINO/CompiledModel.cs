﻿using System;
using System.Collections.Generic;
using Sdcb.OpenVINO.Natives;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents a compiled model in OpenVINO.
/// </summary>
public class CompiledModel : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompiledModel"/> class.
    /// </summary>
    /// <param name="handle">The pointer to the compiled model.</param>
    /// <param name="owned">True if the object should be disposed of when it is no longer needed; otherwise, false.</param>
    public unsafe CompiledModel(IntPtr handle, bool owned = true) : base(handle, owned)
    {
        Inputs = new CompiledInputPortIndexer((ov_compiled_model*)handle);
        Outputs = new CompiledOutputPortIndexer((ov_compiled_model*)handle);
        Properties = new CompiledModelProperties((ov_compiled_model*)handle);
    }

    /// <summary>
    /// Gets the original <see cref="OpenVINO.Model"/> that this <see cref="CompiledModel"/> was created from.
    /// </summary>
    public unsafe Model Model
    {
        get
        {
            ov_model* model;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_get_runtime_model((ov_compiled_model*)Handle, &model));
            return new Model(model, owned: true);
        }
    }

    /// <summary>
    /// Provides an indexer over the input nodes in the model.
    /// </summary>
    public unsafe PortIndexer Inputs { get; }

    /// <summary>
    /// Provides an indexer over the output nodes in the model.
    /// </summary>
    public unsafe PortIndexer Outputs { get; }

    /// <summary>
    /// Creates an <see cref="InferRequest"/> object which is used to infer outputs from inputs in an OpenVINO <see cref="CompiledModel"/>.
    /// </summary>
    /// <returns>An instance of <see cref="InferRequest"/> which is used to infer outputs from inputs in a OpenVINO <see cref="CompiledModel"/>.</returns>
    public unsafe InferRequest CreateInferRequest()
    {
        ov_infer_request* req;
        OpenVINOException.ThrowIfFailed(ov_compiled_model_create_infer_request((ov_compiled_model*)Handle, &req));

        return new InferRequest(req, Inputs.Count, Outputs.Count, owned: true);
    }

    /// <summary>
    /// Gets the properties and their respective values.
    /// </summary>
    /// <remarks>
    /// The properties are retrieved as a space-separated string, split into individual keys, 
    /// and then converted into a dictionary where each key is mapped to its corresponding value.
    /// </remarks>
    /// <returns>
    /// A dictionary where the keys are property names and the values are the corresponding property values.
    /// </returns>
    public unsafe IDictionary<string, string> Properties { get; }

    /// <summary>
    /// Frees the memory associated with the compiled model.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_compiled_model_free((ov_compiled_model*)Handle);
    }
}
