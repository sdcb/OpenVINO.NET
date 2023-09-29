using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.Text;

using static Sdcb.OpenVINO.Natives.NativeMethods;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents the core class for OpenVINO operations, implements IDisposable interface. 
/// </summary>
public class OVCore : CppPtrObject
{
    /// <summary>
    /// Initializes a new instance of the OVCore class with a new handle.
    /// </summary>
    /// <exception cref="OpenVINOException">Thrown when the ov_core_create operation fails.</exception>
    public unsafe OVCore() : this(CreateHandle(), owned: true)
    {
    }

    private unsafe static ov_core* CreateHandle()
    {
        ov_core* core;
        OpenVINOException.ThrowIfFailed(ov_core_create(&core));
        return core;
    }

    /// <summary>
    /// Initializes a new instance of the OVCore class using an existing handle.
    /// </summary>
    /// <param name="ptr">The existing <see cref="ov_core"/> pointer to use.</param>
    /// <param name="owned">Determines if the OVCore instance controls the lifespan of the handle.</param>
    /// <exception cref="ArgumentNullException">Thrown when handle is null.</exception>
    public unsafe OVCore(ov_core* ptr, bool owned) : base((IntPtr)ptr, owned)
    {
    }

    /// <inheritdoc/>
    protected unsafe override void ReleaseCore()
    {
        ov_core* core = (ov_core*)Handle;
        ov_core_free(core);
    }

    /// <summary>
    /// <summary>Get version of OpenVINO.</summary>
    /// </summary>
    public unsafe static OVVersion Version
    {
        get
        {
            ov_version ver = default;
            try
            {
                OpenVINOException.ThrowIfFailed(ov_get_openvino_version(&ver));
                return OVVersion.FromNative(ver);
            }
            finally
            {
                if (ver.buildNumber != null || ver.description != null)
                {
                    ov_version_free(&ver);
                }
            }
        }
    }


    /// <summary>
    /// Retrieves a dictionary of OpenVINO device versions based on the device names.
    /// </summary>
    /// <param name="deviceNames">Device name can be complex and identify multiple devices at once like `HETERO:CPU,GPU`.</param>
    /// <returns>A dictionary mapping device names to their respective versions.</returns>
    public unsafe Dictionary<string, OVVersion> GetDevicePluginsVersions(string deviceNames)
    {
        ov_core_version_list_t vers = default;
        try
        {
            fixed (byte* devicePtr = Encoding.UTF8.GetBytes(deviceNames))
            {
                OpenVINOException.ThrowIfFailed(ov_core_get_versions_by_device_name((ov_core*)Handle, devicePtr, &vers));
            }

            Dictionary<string, OVVersion> result = new(capacity: (int)vers.size);
            for (int i = 0; i < vers.size; ++i)
            {
                ov_core_version_t ver = vers.versions[i];
                string deviceName = StringUtils.UTF8PtrToString((IntPtr)ver.device_name)!;
                OVVersion ovv = OVVersion.FromNative(ver.version);
                result.Add(deviceName, ovv);
            }
            return result;
        }
        finally
        {
            if (vers.size > 0 || vers.versions != null)
            {
                ov_core_versions_free(&vers);
            }
        }
    }

    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats.</summary>
    /// <param name="modelPath">Path to a model.</param>
    /// <param name="binPath">
    /// <para>Path to a data file.</para>
    /// <para>For IR format (*.bin):</para>
    /// <para> * if `bin_path` is empty, will try to read a bin file with the same name as xml and</para>
    /// <para> * if the bin file with the same name is not found, will load IR without weights.</para>
    /// <para>For the following file formats the `bin_path` parameter is not used:</para>
    /// <para> * ONNX format (*.onnx)</para>
    /// <para> * PDPD (*.pdmodel)</para>
    /// <para> * TF (*.pb)</para>
    /// <para> * TFLite (*.tflite)</para>
    /// </param>
    /// <returns>The <see cref="Model"/> that read from specific path.</returns>
    /// <exception cref="ObjectDisposedException" />
    /// <exception cref="OpenVINOException" />
    public unsafe Model ReadModel(string modelPath, string? binPath = null)
    {
        ThrowIfDisposed();
        if (modelPath == null) throw new ArgumentNullException(nameof(modelPath));

        ov_model* model;
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            fixed (char* modelPathPtr = modelPath)
            fixed (char* binPathPtr = binPath)
            {
                OpenVINOException.ThrowIfFailed(ov_core_read_model_unicode((ov_core*)Handle, modelPathPtr, binPathPtr, &model));
            }
        }
        else
        {
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(modelPath))
            fixed (byte* binPathPtr = binPath == null ? null : Encoding.UTF8.GetBytes(binPath))
            {
                OpenVINOException.ThrowIfFailed(ov_core_read_model((ov_core*)Handle, modelPathPtr, binPathPtr, &model));
            }
        }
        return new Model(model, owned: true);
    }

    /// <summary>
    /// <para>Reads a model and creates a compiled model from the IR/ONNX/PDPD file.</para>
    /// <para>This can be more efficient than using the ov_core_read_model_from_XXX + ov_core_compile_model flow,</para>
    /// <para>especially for cases when caching is enabled and a cached model is available.</para>
    /// </summary>
    /// <param name="modelPath">Path to a model.</param>
    /// <param name="deviceName">Name of a device to load a model to.</param>
    /// <returns>The <see cref="Model"/> that read from specific path.</returns>
    /// <exception cref="ObjectDisposedException" />
    /// <exception cref="OpenVINOException" />
    public unsafe CompiledModel CompileModel(string modelPath, string deviceName = "CPU")
    {
        ThrowIfDisposed();
        if (modelPath == null) throw new ArgumentNullException(nameof(modelPath));

        ov_compiled_model* model;
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            fixed (char* modelPathPtr = modelPath)
            fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
            {
                OpenVINOException.ThrowIfFailed(ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, 0, &model, IntPtr.Zero));
            }
        }
        else
        {
            fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(modelPath))
            fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
            {
                OpenVINOException.ThrowIfFailed(ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, 0, &model, IntPtr.Zero));
            }
        }
        return new CompiledModel((IntPtr)model, owned: true);
    }

    /// <summary>
    /// Creates a <see cref="CompiledModel"/> from a source <see cref="Model"/> object.
    /// </summary>
    /// <param name="model">A Model object acquired from <see cref="ReadModel(byte[], Tensor?)"/>. This is the source model object from which the compiled model is created. </param>
    /// <param name="deviceName">Name of a device to load the model to. The default value is "CPU"</param>
    /// <returns>Returns an instance of the <see cref="CompiledModel"/> class.</returns>
    /// <exception cref="OpenVINOException">Throws an exception if compilation of the model fails or if the Handle is null.</exception>

    public unsafe CompiledModel CompileModel(Model model, string deviceName = "CPU")
    {
        ov_compiled_model* cmodel;
        fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
        {
            OpenVINOException.ThrowIfFailed(ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, 0, &cmodel, IntPtr.Zero));
        }
        return new CompiledModel((IntPtr)cmodel, owned: true);
    }

    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats.</summary>
    /// <param name="modelData">Data with a model in IR / ONNX / PDPD / TF / TFLite format.</param>
    /// <param name="weights">Shared pointer to a constant <see cref="Tensor"/> with weights.</param>
    /// <returns>The <see cref="Model"/> that read from memory.</returns>
    /// <exception cref="ObjectDisposedException" />
    /// <exception cref="OpenVINOException" />
    public unsafe Model ReadModel(byte[] modelData, Tensor? weights = null)
    {
        ThrowIfDisposed();
        if (modelData == null) throw new ArgumentNullException(nameof(modelData));

        ov_model* model;
        fixed (byte* modelDataPtr = modelData)
        {
            OpenVINOException.ThrowIfFailed(ov_core_read_model_from_memory((ov_core*)Handle, modelDataPtr, (ov_tensor*)weights?.DangerousGetHandle(), &model));
        }
        return new Model(model, owned: true);
    }
}
