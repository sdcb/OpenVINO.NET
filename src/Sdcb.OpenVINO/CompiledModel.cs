using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        return new InferRequest(req, owned: true);
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
    public unsafe Dictionary<string, string> Properties => GetProperty("SUPPORTED_PROPERTIES")
        .Split(' ')
        .ToDictionary(k => k, GetProperty);


    /// <summary>
    /// Sets a property with the provided key and value.
    /// </summary>
    /// <param name="key">The key of the property to set.</param>
    /// <param name="value">The value to set for the property.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the underlying model is disposed.</exception>
    public unsafe void SetProperty(string key, string value)
    {
        ThrowIfDisposed();

        fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
        fixed (byte* valuePtr = Encoding.UTF8.GetBytes(value + '\0'))
        {
            IntPtr* variadic = stackalloc IntPtr[2];
            variadic[0] = (IntPtr)keyPtr;
            variadic[1] = (IntPtr)valuePtr;
            OpenVINOException.ThrowIfFailed(ov_compiled_model_set_property((ov_compiled_model*)Handle, (IntPtr)variadic));
        }
    }

    /// <summary>
    /// Retrieves a property value based on the provided key.
    /// </summary>
    /// <param name="key">The key of the property to retrieve.</param>
    /// <returns>
    /// The value of the property as a string.
    /// </returns>
    /// <exception cref="ObjectDisposedException">Thrown when the underlying model is disposed.</exception>
    public unsafe string GetProperty(string key)
    {
        ThrowIfDisposed();

        byte* valuePtr;
        fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
        {
            OpenVINOException.ThrowIfFailed(ov_compiled_model_get_property((ov_compiled_model*)Handle, keyPtr, &valuePtr));
            try
            {
                return StringUtils.UTF8PtrToString((IntPtr)valuePtr)!;
            }
            finally
            {
                ov_free(valuePtr);
            }
        }
    }

    /// <summary>
    /// Frees the memory associated with the compiled model.
    /// </summary>
    protected unsafe override void ReleaseCore()
    {
        ov_compiled_model_free((ov_compiled_model*)Handle);
    }
}
