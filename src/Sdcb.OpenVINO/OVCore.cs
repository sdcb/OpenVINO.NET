using Sdcb.OpenVINO.Natives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
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

    private static unsafe ov_core* CreateHandle()
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
    /// Gets an array of string containing the available devices names.
    /// </summary>
    public unsafe string[] AvailableDevices
    {
        get
        {
            ThrowIfDisposed();

            ov_available_devices_t rawDevices;
            try
            {
                OpenVINOException.ThrowIfFailed(ov_core_get_available_devices((ov_core*)Handle, &rawDevices));

                string[] devices = new string[rawDevices.size];
                for (int i = 0; i < devices.Length; ++i)
                {
                    devices[i] = StringUtils.UTF8PtrToString((IntPtr)rawDevices.devices[i])!;
                }

                return devices;
            }
            finally
            {
                ov_available_devices_free(&rawDevices);
            }
        }
    }

    /// <summary>Sets properties for a device, acceptable keys can be found in ov_property_key_xxx.</summary>
    /// <param name="deviceName">Name of a device.</param>
    /// <param name="key">The key of the property to set.</param>
    /// <param name="value">The value to set for the property.</param>
    /// <exception cref="ObjectDisposedException">Thrown when the underlying model is disposed.</exception>
    public unsafe void SetDeviceProperty(string deviceName, string key, string value)
    {
        ThrowIfDisposed();

        fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName + '\0'))
        fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
        fixed (byte* valuePtr = Encoding.UTF8.GetBytes(value + '\0'))
        {
            OpenVINOException.ThrowIfFailed(ov_core_set_property((ov_core*)Handle, deviceNamePtr, (IntPtr)keyPtr, (IntPtr)valuePtr));
        }
    }

    /// <summary>
    /// <para>Gets properties related to device behaviour.</para>
    /// <para>The method extracts information that can be set via the <see cref="SetDeviceProperty"/> method.</para>
    /// </summary>
    /// <param name="deviceName"> Name of a device to get a property value.</param>
    /// <param name="key">The key of the property to retrieve.</param>
    /// <returns>
    /// The value of the property as a string.
    /// </returns>
    /// <exception cref="ObjectDisposedException">Thrown when the underlying model is disposed.</exception>
    public unsafe string GetProperty(string deviceName, string key)
    {
        ThrowIfDisposed();

        byte* valuePtr;
        fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName + '\0'))
        fixed (byte* keyPtr = Encoding.UTF8.GetBytes(key + '\0'))
        {
            OpenVINOException.ThrowIfFailed(ov_core_get_property((ov_core*)Handle, deviceNamePtr, keyPtr, &valuePtr));
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
        if (!OpenVINOLibraryLoader.Is202302OrGreater())
        {
            if (!File.Exists(modelPath)) throw new FileNotFoundException($"Model path not found: {modelPath}", modelPath);
        }

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
    /// <param name="properties">Properties to configure the <see cref="CompiledModel"/></param>
    /// <returns>The <see cref="Model"/> that read from specific path.</returns>
    /// <exception cref="ObjectDisposedException" />
    /// <exception cref="OpenVINOException" />
    public unsafe CompiledModel CompileModel(string modelPath, string deviceName = DeviceOptions.DefaultDeviceName, Dictionary<string, string>? properties = null)
    {
        ThrowIfDisposed();
        if (modelPath == null) throw new ArgumentNullException(nameof(modelPath));
        if (!OpenVINOLibraryLoader.Is202302OrGreater())
        {
            if (!File.Exists(modelPath)) throw new FileNotFoundException($"Model path not found: {modelPath}", modelPath);
        }

        properties ??= new();
        GCHandle[] gchs = new GCHandle[properties.Count * 2];
        try
        {
            IntPtr* v = stackalloc IntPtr[properties.Count * 2 + 1];

            {
                // convert properties to variadic
                int i = 0;
                foreach (KeyValuePair<string, string> kvp in properties)
                {
                    GCHandle keyGch = GCHandle.Alloc(Encoding.UTF8.GetBytes(kvp.Key + '\0'), GCHandleType.Pinned);
                    v[i] = keyGch.AddrOfPinnedObject();
                    gchs[i] = keyGch;

                    GCHandle valueGch = GCHandle.Alloc(Encoding.UTF8.GetBytes(kvp.Value + '\0'), GCHandleType.Pinned);
                    v[i + 1] = valueGch.AddrOfPinnedObject();
                    gchs[i + 1] = valueGch;

                    i += 2;
                }
                v[i] = IntPtr.Zero;
            }

            ov_compiled_model* model;
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                fixed (char* modelPathPtr = modelPath)
                fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
                {
                    OpenVINOException.ThrowIfFailed(properties.Count switch
                    {
                        0 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model),
                        1 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1]),
                        2 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3]),
                        3 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5]),
                        4 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7]),
                        5 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9]),
                        6 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11]),
                        7 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13]),
                        8 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15]),
                        9 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17]),
                        10 => ov_core_compile_model_from_file_unicode((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17], v[18], v[19]),
                        _ => throw new ArgumentOutOfRangeException(nameof(properties), $"Properties count > 10 not supported, provided: {properties.Count}")
                    });
                }
            }
            else
            {
                fixed (byte* modelPathPtr = Encoding.UTF8.GetBytes(modelPath))
                fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
                {
                    OpenVINOException.ThrowIfFailed(properties.Count switch
                    {
                        0 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model),
                        1 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1]),
                        2 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3]),
                        3 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5]),
                        4 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7]),
                        5 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9]),
                        6 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11]),
                        7 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13]),
                        8 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15]),
                        9 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17]),
                        10 => ov_core_compile_model_from_file((ov_core*)Handle, modelPathPtr, deviceNamePtr, properties.Count * 2, &model, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17], v[18], v[19]),
                        _ => throw new ArgumentOutOfRangeException(nameof(properties), $"Properties count > 10 not supported, provided: {properties.Count}")
                    });
                }
            }
            return new CompiledModel((IntPtr)model, owned: true);
        }
        finally
        {
            foreach (GCHandle gch in gchs)
            {
                if (gch.IsAllocated)
                {
                    gch.Free();
                }
            }
        }
    }

    /// <summary>
    /// Creates a <see cref="CompiledModel"/> from a source <see cref="Model"/> object.
    /// </summary>
    /// <param name="model">A Model object acquired from <see cref="ReadModel(byte[], Tensor?)"/>. This is the source model object from which the compiled model is created. </param>
    /// <param name="deviceName">Name of a device to load the model to. The default value is <see cref="DeviceOptions.DefaultDeviceName"/></param>
    /// <param name="properties">Properties to configure the <see cref="CompiledModel"/></param>
    /// <returns>Returns an instance of the <see cref="CompiledModel"/> class.</returns>
    /// <exception cref="OpenVINOException">Throws an exception if compilation of the model fails or if the Handle is null.</exception>

    public unsafe CompiledModel CompileModel(Model model, string deviceName = DeviceOptions.DefaultDeviceName, Dictionary<string, string>? properties = null)
    {
        properties ??= new();
        GCHandle[] gCHandles = new GCHandle[properties.Count * 2];
        try
        {
            IntPtr* v = stackalloc IntPtr[properties.Count * 2];
            {
                // convert properties to variadic
                int i = 0;
                foreach (KeyValuePair<string, string> kvp in properties)
                {
                    GCHandle keyGch = GCHandle.Alloc(Encoding.UTF8.GetBytes(kvp.Key + '\0'), GCHandleType.Pinned);
                    v[i++] = keyGch.AddrOfPinnedObject();

                    GCHandle valueGch = GCHandle.Alloc(Encoding.UTF8.GetBytes(kvp.Value + '\0'), GCHandleType.Pinned);
                    v[i++] = valueGch.AddrOfPinnedObject();
                }
            }

            ov_compiled_model* cmodel;
            fixed (byte* deviceNamePtr = Encoding.UTF8.GetBytes(deviceName))
            {
                OpenVINOException.ThrowIfFailed(properties.Count switch
                {
                    0 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel),
                    1 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1]),
                    2 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3]),
                    3 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5]),
                    4 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7]),
                    5 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9]),
                    6 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11]),
                    7 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13]),
                    8 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15]),
                    9 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17]),
                    10 => ov_core_compile_model((ov_core*)Handle, (ov_model*)model.DangerousGetHandle(), deviceNamePtr, properties.Count * 2, &cmodel, v[0], v[1], v[2], v[3], v[4], v[5], v[6], v[7], v[8], v[9], v[10], v[11], v[12], v[13], v[14], v[15], v[16], v[17], v[18], v[19]),
                    _ => throw new ArgumentOutOfRangeException(nameof(properties), $"Properties count > 10 not supported, provided: {properties.Count}")
                });
            }
            return new CompiledModel((IntPtr)cmodel, owned: true);
        }
        finally
        {
            foreach (GCHandle gch in gCHandles)
            {
                if (gch.IsAllocated)
                {
                    gch.Free();
                }
            }
        }
    }

    /// <summary>
    /// Compiles the specified modelPath into a binary using deviceOptions.
    /// </summary>
    /// <param name="modelPath">The path to the model to compile.</param>
    /// <param name="deviceOptions">The device options (device name and a dictionary of properties) to use while compiling.</param>
    /// <returns>The compiled model.</returns>
    public CompiledModel CompileModel(string modelPath, DeviceOptions deviceOptions)
    {
        return CompileModel(modelPath, deviceOptions.DeviceName, deviceOptions.Properties);
    }

    /// <summary>
    /// Compiles the specified model into a binary using deviceOptions.
    /// </summary>
    /// <param name="model">The Model object to compile.</param>
    /// <param name="deviceOptions">The device options (device name and a dictionary of properties) to use while compiling.</param>
    /// <returns>The compiled model.</returns>
    public CompiledModel CompileModel(Model model, DeviceOptions deviceOptions)
    {
        return CompileModel(model, deviceOptions.DeviceName, deviceOptions.Properties);
    }

    /// <summary>
    /// Reads models from IR / ONNX / PDPD / TF / TFLite formats. 
    /// <para>
    /// Starting from version 2023.2, this method will invoke a newer interface <see cref="ov_core_read_model_from_memory_buffer"/>,
    /// which supports models including '\0' character in the file. This is particularly useful for binary format models like PaddlePaddle, ONNX, etc.
    /// </para>
    /// <para>In previous versions, such binary files were interpreted as strings, leading to an early termination at encountering '\0', thus limiting the scope of usage.</para>
    /// </summary>
    /// <param name="modelData">Data with a model in IR / ONNX / PDPD / TF / TFLite format.</param>
    /// <param name="weights">Shared pointer to a constant <see cref="Tensor"/> with weights.</param>
    /// <returns>The <see cref="Model"/> that is read from memory.</returns>
    /// <exception cref="ObjectDisposedException" />
    /// <exception cref="OpenVINOException" />
    public unsafe Model ReadModel(byte[] modelData, Tensor? weights = null)
    {
        ThrowIfDisposed();
        if (modelData == null) throw new ArgumentNullException(nameof(modelData));

        ov_model* model;
        fixed (byte* modelDataPtr = modelData)
        {
            if (OpenVINOLibraryLoader.Is202302OrGreater())
            {
                OpenVINOException.ThrowIfFailed(ov_core_read_model_from_memory_buffer(
                    (ov_core*)Handle, modelDataPtr, modelData.Length, (ov_tensor*)weights?.DangerousGetHandle(), &model));
            }
            else
            {
                OpenVINOException.ThrowIfFailed(ov_core_read_model_from_memory(
                    (ov_core*)Handle, modelDataPtr, (ov_tensor*)weights?.DangerousGetHandle(), &model));
            }
        }
        return new Model(model, owned: true);
    }

    /// <summary>
    /// Reads models from IR / ONNX / PDPD / TF / TFLite formats using model data and optional weights data. Starting from version 2023.2, this method will invoke a newer interface 'ov_core_read_model_from_memory_buffer',
    /// which supports models including '\0' character in the file. This is particularly useful for binary format models like PaddlePaddle, ONNX, etc.
    /// In earlier versions, such binary files were interpreted as strings leading to an early termination at encountering '\0', thus limiting the scope of usage. 
    /// </summary>
    /// <param name="modelData">ByteArray with a model in IR / ONNX / PDPD / TF / TFLite format.</param>
    /// <param name="weightsData">Optional byte array of weights data. If provided, this will be converted to a Tensor.</param>
    /// <returns>The <see cref="Model"/> that is read from memory.</returns>
    /// <exception cref="ObjectDisposedException" />
    public unsafe Model ReadModel(byte[] modelData, byte[]? weightsData = null)
    {
        ThrowIfDisposed();

        Tensor? weights = null;
        if (weightsData != null)
        {
            weights = Tensor.FromByteArray(weightsData);
        }

        using Tensor? _ = weights;
        return ReadModel(modelData, weights);
    }

    /// <summary>
    /// Creates a weak reference to this OVCore object.
    /// </summary>
    /// <returns>A weak reference to this OVCore object.</returns>
    public unsafe OVCore WeakRef()
    {
        ThrowIfDisposed();
        return new OVCore((ov_core*)Handle, owned: false);
    }

    private unsafe static Lazy<OVCore> _sharedCore = new(() => new OVCore());

    /// <summary>
    /// Gets the shared instance of the <see cref="OVCore"/> class.
    /// </summary>
    public static OVCore Shared => _sharedCore.Value.WeakRef();

    /// <summary>
    /// Disposes the shared handle.
    /// </summary>
    public unsafe static void DisposeSharedInstance()
    {
        if (_sharedCore.IsValueCreated)
        {
            OVCore core = _sharedCore.Value;
            core.Dispose();
            _sharedCore = new Lazy<OVCore>(() => new OVCore());
        }
    }

    /// <summary>
    /// Shut down the OpenVINO by deleting all static-duration objects allocated by the library and releasing dependent resources
    /// </summary>
    public static void Shutdown()
    {
        ov_shutdown();
    }
}

