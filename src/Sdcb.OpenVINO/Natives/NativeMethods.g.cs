#pragma warning disable CS1591
#pragma warning disable CS1573
using System;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;

public static unsafe partial class NativeMethods
{
    

    /// <summary>Print the error info.</summary>
    /// <param name="status">a status code.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_common.h", 225, 226, "ov_base_c_api")]
    public static extern byte* ov_get_error_info(ov_status_e status);
    

    /// <summary>free char</summary>
    /// <param name="content">The pointer to the char to free.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_common.h", 233, 234, "ov_base_c_api")]
    public static extern void ov_free(byte* content);
    

    /// <summary>Get the last error msg.</summary>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_common.h", 240, 241, "ov_base_c_api")]
    public static extern byte* ov_get_last_err_msg();
    

    /// <summary>Get the input size of ov_compiled_model_t.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="size">the compiled_model&apos;s input size.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 35, 36, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_inputs_size(ov_compiled_model* compiled_model, nint* size);
    

    /// <summary>Get the single const input port of ov_compiled_model_t, which only support single input model.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 45, 46, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_input(ov_compiled_model* compiled_model, ov_output_const_port** input_port);
    

    /// <summary>Get a const input port of ov_compiled_model_t by port index.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="index">input index.</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 56, 59, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_input_by_index(ov_compiled_model* compiled_model, nint index, ov_output_const_port** input_port);
    

    /// <summary>Get a const input port of ov_compiled_model_t by name.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="name">input tensor name (char *).</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 69, 72, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_input_by_name(ov_compiled_model* compiled_model, byte* name, ov_output_const_port** input_port);
    

    /// <summary>Get the output size of ov_compiled_model_t.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="size">the compiled_model&apos;s output size.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 81, 82, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_outputs_size(ov_compiled_model* compiled_model, nint* size);
    

    /// <summary>Get the single const output port of ov_compiled_model_t, which only support single output model.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 91, 92, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_output(ov_compiled_model* compiled_model, ov_output_const_port** output_port);
    

    /// <summary>Get a const output port of ov_compiled_model_t by port index.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="index">input index.</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 102, 105, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_output_by_index(ov_compiled_model* compiled_model, nint index, ov_output_const_port** output_port);
    

    /// <summary>Get a const output port of ov_compiled_model_t by name.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="name">input tensor name (char *).</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 115, 118, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_output_by_name(ov_compiled_model* compiled_model, byte* name, ov_output_const_port** output_port);
    

    /// <summary>Gets runtime model information from a device.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 127, 128, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_get_runtime_model(ov_compiled_model* compiled_model, ov_model** model);
    

    /// <summary>Creates an inference request object used to infer the compiled model.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 137, 138, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_create_infer_request(ov_compiled_model* compiled_model, ov_infer_request** infer_request);
    

    /// <summary>Sets properties for a device, acceptable keys can be found in ov_property_key_xxx.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <remarks>
    /// <para>variadic paramaters The format is &lt;char *property_key, char* property_value&gt;.</para>
    /// <para>Supported property key please see ov_property.h.</para>
    /// </remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 148, 149, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_set_property(ov_compiled_model* compiled_model);
    [DllImport(Dll, EntryPoint = nameof(ov_compiled_model_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_compiled_model_set_property(ov_compiled_model* compiled_model, IntPtr key, IntPtr value);
    

    /// <summary>Gets properties for current compiled model.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="property_key">Property key.</param>
    /// <param name="property_value">A pointer to property value.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 159, 162, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_get_property(ov_compiled_model* compiled_model, byte* property_key, byte** property_value);
    

    /// <summary>
    /// <para>Exports the current compiled model to an output stream `std::ostream`.</para>
    /// <para>The exported model can also be imported via the ov::Core::import_model method.</para>
    /// </summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="export_model_path">Path to the file.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 172, 173, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_export_model(ov_compiled_model* compiled_model, byte* export_model_path);
    

    /// <summary>Release the memory allocated by ov_compiled_model_t.</summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 180, 181, "ov_compiled_model_c_api")]
    public static extern void ov_compiled_model_free(ov_compiled_model* compiled_model);
    

    /// <summary>
    /// <para>Returns pointer to device-specific shared context</para>
    /// <para>on a remote accelerator device that was used to create this CompiledModel.</para>
    /// </summary>
    /// <param name="compiled_model">A pointer to the ov_compiled_model_t.</param>
    /// <param name="context">Return context.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_compiled_model.h", 192, 193, "ov_compiled_model_c_api")]
    public static extern ov_status_e ov_compiled_model_get_context(ov_compiled_model* compiled_model, ov_remote_context** context);
    

    /// <summary>Get version of OpenVINO.</summary>
    /// <param name="version">a pointer to the version</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 75, 76, "ov_core_c_api")]
    public static extern ov_status_e ov_get_openvino_version(ov_version* version);
    

    /// <summary>Release the memory allocated by ov_version_t.</summary>
    /// <param name="version">A pointer to the ov_version_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 83, 84, "ov_core_c_api")]
    public static extern void ov_version_free(ov_version* version);
    

    /// <summary>
    /// <para>Constructs OpenVINO Core instance by default.</para>
    /// <para>See RegisterPlugins for more details.</para>
    /// </summary>
    /// <param name="core">A pointer to the newly created ov_core_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 93, 94, "ov_core_c_api")]
    public static extern ov_status_e ov_core_create(ov_core** core);
    

    /// <summary>
    /// <para>Constructs OpenVINO Core instance using XML configuration file with devices description.</para>
    /// <para>See RegisterPlugins for more details.</para>
    /// </summary>
    /// <param name="xml_config_file">
    /// <para>A path to .xml file with devices to load from. If XML configuration file is not specified,</para>
    /// <para>then default plugin.xml file will be used.</para>
    /// </param>
    /// <param name="core">A pointer to the newly created ov_core_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 105, 106, "ov_core_c_api")]
    public static extern ov_status_e ov_core_create_with_config(byte* xml_config_file, ov_core** core);
    

    /// <summary>
    /// <para>Constructs OpenVINO Core instance.</para>
    /// <para>See RegisterPlugins for more details.</para>
    /// </summary>
    /// <param name="xml_config_file_ws">A path to model file with unicode.</param>
    /// <param name="core">A pointer to the newly created ov_core_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 117, 118, "ov_core_c_api")]
    public static extern ov_status_e ov_core_create_with_config_unicode(int* xml_config_file_ws, ov_core** core);
    

    /// <summary>Release the memory allocated by ov_core_t.</summary>
    /// <param name="core">A pointer to the ov_core_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 126, 127, "ov_core_c_api")]
    public static extern void ov_core_free(ov_core* core);
    

    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model_path">Path to a model.</param>
    /// <param name="bin_path">
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
    /// <param name="model">A pointer to the newly created model.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 146, 147, "ov_core_c_api")]
    public static extern ov_status_e ov_core_read_model(ov_core* core, byte* model_path, byte* bin_path, ov_model** model);
    

    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats, path is unicode.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model_path">Path to a model.</param>
    /// <param name="bin_path">
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
    /// <param name="model">A pointer to the newly created model.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 167, 171, "ov_core_c_api")]
    public static extern ov_status_e ov_core_read_model_unicode(ov_core* core, int* model_path, int* bin_path, ov_model** model);
    

    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats with models string size.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model_str">
    /// <para>String with a model in IR / ONNX / PDPD / TF / TFLite format, support model string containing</para>
    /// <para>several null chars.</para>
    /// </param>
    /// <param name="str_len">The length of model string.</param>
    /// <param name="weights">Shared pointer to a constant tensor with weights.</param>
    /// <param name="model">
    /// <para>A pointer to the newly created model.</para>
    /// <para>Reading ONNX / PDPD / TF / TFLite models does not support loading weights from the @p weights tensors.</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 189, 194, "ov_core_c_api")]
    public static extern ov_status_e ov_core_read_model_from_memory_buffer(ov_core* core, byte* model_str, nint str_len, ov_tensor* weights, ov_model** model);
    

    /// <summary>
    /// <para>Creates a compiled model from a source model object.</para>
    /// <para>Users can create as many compiled models as they need and use</para>
    /// <para>them simultaneously (up to the limitation of the hardware resources).</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model">Model object acquired from Core::read_model.</param>
    /// <param name="device_name">Name of a device to load a model to.</param>
    /// <param name="property_args_size">How many properties args will be passed, each property contains 2 args: key and value.</param>
    /// <param name="compiled_model">A pointer to the newly created compiled_model.</param>
    /// <remarks>
    /// <para>property paramater: Optional pack of pairs: &lt;char* property_key, char* property_value&gt; relevant only</para>
    /// <para>for this load operation operation. Supported property key please see ov_property.h.</para>
    /// </remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 210, 216, "ov_core_c_api")]
    public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model(ov_core* core, ov_model* model, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>
    /// <para>Reads a model and creates a compiled model from the IR/ONNX/PDPD file.</para>
    /// <para>This can be more efficient than using the ov_core_read_model_from_XXX + ov_core_compile_model flow,</para>
    /// <para>especially for cases when caching is enabled and a cached model is available.</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model_path">Path to a model.</param>
    /// <param name="device_name">Name of a device to load a model to.</param>
    /// <param name="property_args_size">How many properties args will be passed, each property contains 2 args: key and value.</param>
    /// <param name="compiled_model">A pointer to the newly created compiled_model.</param>
    /// <remarks>
    /// <para>Optional pack of pairs: &lt;char* property_key, char* property_value&gt; relevant only</para>
    /// <para>for this load operation operation. Supported property key please see ov_property.h.</para>
    /// </remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 232, 238, "ov_core_c_api")]
    public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file(ov_core* core, byte* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>
    /// <para>Reads a model and creates a compiled model from the IR/ONNX/PDPD file.</para>
    /// <para>This can be more efficient than using the ov_core_read_model_from_XXX + ov_core_compile_model flow,</para>
    /// <para>especially for cases when caching is enabled and a cached model is available.</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model_path">Path to a model.</param>
    /// <param name="device_name">Name of a device to load a model to.</param>
    /// <param name="property_args_size">How many properties args will be passed, each property contains 2 args: key and value.</param>
    /// <param name="compiled_model">A pointer to the newly created compiled_model.</param>
    /// <remarks>
    /// <para>Optional pack of pairs: &lt;char* property_key, char* property_value&gt; relevant only</para>
    /// <para>for this load operation operation. Supported property key please see ov_property.h.</para>
    /// </remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 255, 261, "ov_core_c_api")]
    public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_from_file_unicode), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_from_file_unicode(ov_core* core, int* model_path, byte* device_name, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>Sets properties for a device, acceptable keys can be found in ov_property_key_xxx.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="device_name">Name of a device.</param>
    /// <remarks>
    /// <para>variadic paramaters The format is &lt;char* property_key, char* property_value&gt;.</para>
    /// <para>Supported property key please see ov_property.h.</para>
    /// </remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 274, 275, "ov_core_c_api")]
    public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_set_property), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_set_property(ov_core* core, byte* device_name, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>
    /// <para>Gets properties related to device behaviour.</para>
    /// <para>The method extracts information that can be set via the set_property method.</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="device_name"> Name of a device to get a property value.</param>
    /// <param name="property_key"> Property key.</param>
    /// <param name="property_value">A pointer to property value with string format.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 287, 288, "ov_core_c_api")]
    public static extern ov_status_e ov_core_get_property(ov_core* core, byte* device_name, byte* property_key, byte** property_value);
    

    /// <summary>Returns devices available for inference.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="devices">
    /// <para>A pointer to the ov_available_devices_t instance.</para>
    /// <para>Core objects go over all registered plugins and ask about available devices.</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 298, 299, "ov_core_c_api")]
    public static extern ov_status_e ov_core_get_available_devices(ov_core* core, ov_available_devices_t* devices);
    

    /// <summary>Releases memory occpuied by ov_available_devices_t</summary>
    /// <param name="devices">A pointer to the ov_available_devices_t instance.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 307, 308, "ov_core_c_api")]
    public static extern void ov_available_devices_free(ov_available_devices_t* devices);
    

    /// <summary>Imports a compiled model from the previously exported one.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="content">A pointer to content of the exported model.</param>
    /// <param name="content_size">Number of bytes in the exported network.</param>
    /// <param name="device_name">Name of a device to import a compiled model for.</param>
    /// <param name="compiled_model">A pointer to the newly created compiled_model.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 320, 325, "ov_core_c_api")]
    public static extern ov_status_e ov_core_import_model(ov_core* core, byte* content, nint content_size, byte* device_name, ov_compiled_model** compiled_model);
    

    /// <summary>
    /// <para>Returns device plugins version information.</para>
    /// <para>Device name can be complex and identify multiple devices at once like `HETERO:CPU,GPU`;</para>
    /// <para>in this case, std::map contains multiple entries, each per device.</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="device_name">Device name to identify a plugin.</param>
    /// <param name="versions">A pointer to versions corresponding to device_name.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 337, 338, "ov_core_c_api")]
    public static extern ov_status_e ov_core_get_versions_by_device_name(ov_core* core, byte* device_name, ov_core_version_list_t* versions);
    

    /// <summary>Releases memory occupied by ov_core_version_list_t.</summary>
    /// <param name="versions">A pointer to the ov_core_version_list_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 345, 346, "ov_core_c_api")]
    public static extern void ov_core_versions_free(ov_core_version_list_t* versions);
    

    /// <summary>
    /// <para>Creates a new remote shared context object on the specified accelerator device</para>
    /// <para>using specified plugin-specific low-level device API parameters (device handle, pointer, context, etc.).</para>
    /// </summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="device_name">Device name to identify a plugin.</param>
    /// <param name="context_args_size">How many property args will be for this remote context creation.</param>
    /// <param name="context">A pointer to the newly created remote context.</param>
    /// <remarks>variadic parmameters Actual context property parameter for remote context</remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 359, 364, "ov_core_c_api")]
    public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_create_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_create_context(ov_core* core, byte* device_name, nint context_args_size, ov_remote_context** context, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>Creates a compiled model from a source model within a specified remote context.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="model">Model object acquired from ov_core_read_model.</param>
    /// <param name="context">A pointer to the newly created remote context.</param>
    /// <param name="property_args_size">How many args will be for this compiled model.</param>
    /// <param name="compiled_model">A pointer to the newly created compiled_model.</param>
    /// <remarks>variadic parmameters Actual property parameter for remote context</remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 377, 383, "ov_core_c_api")]
    public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_core_compile_model_with_context), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_core_compile_model_with_context(ov_core* core, ov_model* model, ov_remote_context* context, nint property_args_size, ov_compiled_model** compiled_model, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>Gets a pointer to default (plugin-supplied) shared context object for the specified accelerator device.</summary>
    /// <param name="core">A pointer to the ov_core_t instance.</param>
    /// <param name="device_name">Name of a device to get a default shared context from.</param>
    /// <param name="context">A pointer to the referenced remote context.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 393, 394, "ov_core_c_api")]
    public static extern ov_status_e ov_core_get_default_context(ov_core* core, byte* device_name, ov_remote_context** context);
    

    /// <summary>
    /// <para>Shut down the OpenVINO by deleting all static-duration objects allocated by the library and releasing</para>
    /// <para>dependent resources</para>
    /// </summary>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 405, 405, "ov_c_api")]
    public static extern void ov_shutdown();
    

    /// <summary>Check this dimension whether is dynamic</summary>
    /// <param name="dim">The dimension pointer that will be checked.</param>
    /// <returns>Boolean, true is dynamic and false is static.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_dimension.h", 31, 32, "ov_dimension_c_api")]
    public static extern bool ov_dimension_is_dynamic(ov_dimension dim);
    

    /// <summary>Set an input/output tensor to infer on by the name of tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor_name"> Name of the input or output tensor.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 70, 71, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_tensor(ov_infer_request* infer_request, byte* tensor_name, ov_tensor* tensor);
    

    /// <summary>Set an input/output tensor to infer request for the port.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="port">Port of the input or output tensor, which can be got by calling ov_model_t/ov_compiled_model_t interface.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 81, 84, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_tensor_by_port(ov_infer_request* infer_request, ov_output_port* port, ov_tensor* tensor);
    

    /// <summary>Set an input/output tensor to infer request for the port.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="port">
    /// <para>Const port of the input or output tensor, which can be got by call interface from</para>
    /// <para>ov_model_t/ov_compiled_model_t.</para>
    /// </param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 95, 98, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_tensor_by_const_port(ov_infer_request* infer_request, ov_output_const_port* port, ov_tensor* tensor);
    

    /// <summary>Set an input tensor to infer on by the index of tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="idx">Index of the input port. If @p idx is greater than the number of model inputs, an error will return.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 108, 111, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_input_tensor_by_index(ov_infer_request* infer_request, nint idx, ov_tensor* tensor);
    

    /// <summary>Set an input tensor for the model with single input to infer on.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 121, 122, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_input_tensor(ov_infer_request* infer_request, ov_tensor* tensor);
    

    /// <summary>Set an output tensor to infer by the index of output tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="idx">Index of the output tensor.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 133, 136, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_output_tensor_by_index(ov_infer_request* infer_request, nint idx, ov_tensor* tensor);
    

    /// <summary>Set an output tensor to infer models with single output.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 145, 146, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_output_tensor(ov_infer_request* infer_request, ov_tensor* tensor);
    

    /// <summary>Get an input/output tensor by the name of tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor_name">Name of the input or output tensor to get.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 157, 158, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_tensor(ov_infer_request* infer_request, byte* tensor_name, ov_tensor** tensor);
    

    /// <summary>Get an input/output tensor by const port.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="port">Port of the tensor to get. @p port is not found, an error will return.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 168, 171, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_tensor_by_const_port(ov_infer_request* infer_request, ov_output_const_port* port, ov_tensor** tensor);
    

    /// <summary>Get an input/output tensor by port.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="port">Port of the tensor to get. @p port is not found, an error will return.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 181, 184, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_tensor_by_port(ov_infer_request* infer_request, ov_output_port* port, ov_tensor** tensor);
    

    /// <summary>Get an input tensor by the index of input tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="idx">
    /// <para>Index of the tensor to get. @p idx. If the tensor with the specified @p idx is not found, an error will</para>
    /// <para>return.</para>
    /// </param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 195, 198, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_input_tensor_by_index(ov_infer_request* infer_request, nint idx, ov_tensor** tensor);
    

    /// <summary>Get an input tensor from the model with only one input tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 207, 208, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_input_tensor(ov_infer_request* infer_request, ov_tensor** tensor);
    

    /// <summary>Get an output tensor by the index of output tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="idx">
    /// <para>Index of the tensor to get. @p idx. If the tensor with the specified @p idx is not found, an error will</para>
    /// <para>return.</para>
    /// </param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 219, 222, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_output_tensor_by_index(ov_infer_request* infer_request, nint idx, ov_tensor** tensor);
    

    /// <summary>Get an output tensor from the model with only one output tensor.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="tensor">Reference to the tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 232, 233, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_output_tensor(ov_infer_request* infer_request, ov_tensor** tensor);
    

    /// <summary>Infer specified input(s) in synchronous mode.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 241, 242, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_infer(ov_infer_request* infer_request);
    

    /// <summary>Cancel inference request.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 250, 251, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_cancel(ov_infer_request* infer_request);
    

    /// <summary>Start inference of specified input(s) in asynchronous mode.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 259, 260, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_start_async(ov_infer_request* infer_request);
    

    /// <summary>Wait for the result to become available.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 268, 269, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_wait(ov_infer_request* infer_request);
    

    /// <summary>
    /// <para>Waits for the result to become available. Blocks until the specified timeout has elapsed or the result</para>
    /// <para>becomes available, whichever comes first.</para>
    /// </summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="timeout">Maximum duration, in milliseconds, to block for.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 279, 280, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_wait_for(ov_infer_request* infer_request, long timeout);
    

    /// <summary>Set callback function, which will be called when inference is done.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="callback"> A function to be called.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 289, 290, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_set_callback(ov_infer_request* infer_request, ov_callback_t* callback);
    

    /// <summary>Release the memory allocated by ov_infer_request_t.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 297, 298, "ov_infer_request_c_api")]
    public static extern void ov_infer_request_free(ov_infer_request* infer_request);
    

    /// <summary>Query performance measures per layer to identify the most time consuming operation.</summary>
    /// <param name="infer_request">A pointer to the ov_infer_request_t.</param>
    /// <param name="profiling_infos"> Vector of profiling information for operations in a model.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 307, 308, "ov_infer_request_c_api")]
    public static extern ov_status_e ov_infer_request_get_profiling_info(ov_infer_request* infer_request, ov_profiling_info_list_t* profiling_infos);
    

    /// <summary>Release the memory allocated by ov_profiling_info_list_t.</summary>
    /// <param name="profiling_infos">A pointer to the ov_profiling_info_list_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_infer_request.h", 315, 316, "ov_infer_request_c_api")]
    public static extern void ov_profiling_info_list_free(ov_profiling_info_list_t* profiling_infos);
    

    /// <summary>Create a layout object.</summary>
    /// <param name="layout_desc">The layout input pointer.</param>
    /// <param name="layout">The description of layout.</param>
    /// <returns>ov_status_e a status code, return OK if successful</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_layout.h", 29, 30, "ov_layout_c_api")]
    public static extern ov_status_e ov_layout_create(byte* layout_desc, ov_layout** layout);
    

    /// <summary>Free layout object.</summary>
    /// <param name="layout">will be released.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_layout.h", 37, 38, "ov_layout_c_api")]
    public static extern void ov_layout_free(ov_layout* layout);
    

    /// <summary>Convert layout object to a readable string.</summary>
    /// <param name="layout">will be converted.</param>
    /// <returns>string that describes the layout content.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_layout.h", 46, 47, "ov_layout_c_api")]
    public static extern byte* ov_layout_to_string(ov_layout* layout);
    

    /// <summary>Release the memory allocated by ov_model_t.</summary>
    /// <param name="model">A pointer to the ov_model_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 29, 30, "ov_model_c_api")]
    public static extern void ov_model_free(ov_model* model);
    

    /// <summary>Get a const input port of ov_model_t,which only support single input model.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 39, 40, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_input(ov_model* model, ov_output_const_port** input_port);
    

    /// <summary>Get a const input port of ov_model_t by name.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_name">The name of input tensor.</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 50, 51, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_input_by_name(ov_model* model, byte* tensor_name, ov_output_const_port** input_port);
    

    /// <summary>Get a const input port of ov_model_t by port index.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="index">input tensor index.</param>
    /// <param name="input_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 61, 62, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_input_by_index(ov_model* model, nint index, ov_output_const_port** input_port);
    

    /// <summary>Get single input port of ov_model_t, which only support single input model.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="input_port">A pointer to the ov_output_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 71, 72, "ov_model_c_api")]
    public static extern ov_status_e ov_model_input(ov_model* model, ov_output_port** input_port);
    

    /// <summary>Get an input port of ov_model_t by name.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_name">input tensor name (char *).</param>
    /// <param name="input_port">A pointer to the ov_output_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 82, 83, "ov_model_c_api")]
    public static extern ov_status_e ov_model_input_by_name(ov_model* model, byte* tensor_name, ov_output_port** input_port);
    

    /// <summary>Get an input port of ov_model_t by port index.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="index">input tensor index.</param>
    /// <param name="input_port">A pointer to the ov_output_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 93, 94, "ov_model_c_api")]
    public static extern ov_status_e ov_model_input_by_index(ov_model* model, nint index, ov_output_port** input_port);
    

    /// <summary>Get a single const output port of ov_model_t, which only support single output model.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 103, 104, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_output(ov_model* model, ov_output_const_port** output_port);
    

    /// <summary>Get a const output port of ov_model_t by port index.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="index">input tensor index.</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 114, 115, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_output_by_index(ov_model* model, nint index, ov_output_const_port** output_port);
    

    /// <summary>Get a const output port of ov_model_t by name.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_name">input tensor name (char *).</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 125, 126, "ov_model_c_api")]
    public static extern ov_status_e ov_model_const_output_by_name(ov_model* model, byte* tensor_name, ov_output_const_port** output_port);
    

    /// <summary>Get a single output port of ov_model_t, which only support single output model.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="output_port">A pointer to the ov_output_const_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 135, 136, "ov_model_c_api")]
    public static extern ov_status_e ov_model_output(ov_model* model, ov_output_port** output_port);
    

    /// <summary>Get an output port of ov_model_t by port index.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="index">input tensor index.</param>
    /// <param name="output_port">A pointer to the ov_output_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 146, 147, "ov_model_c_api")]
    public static extern ov_status_e ov_model_output_by_index(ov_model* model, nint index, ov_output_port** output_port);
    

    /// <summary>Get an output port of ov_model_t by name.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_name">output tensor name (char *).</param>
    /// <param name="output_port">A pointer to the ov_output_port_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 157, 158, "ov_model_c_api")]
    public static extern ov_status_e ov_model_output_by_name(ov_model* model, byte* tensor_name, ov_output_port** output_port);
    

    /// <summary>Get the input size of ov_model_t.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="input_size">the model&apos;s input size.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 167, 168, "ov_model_c_api")]
    public static extern ov_status_e ov_model_inputs_size(ov_model* model, nint* input_size);
    

    /// <summary>Get the output size of ov_model_t.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="output_size">the model&apos;s output size.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 177, 178, "ov_model_c_api")]
    public static extern ov_status_e ov_model_outputs_size(ov_model* model, nint* output_size);
    

    /// <summary>Returns true if any of the ops defined in the model is dynamic shape.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <returns>true if model contains dynamic shapes</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 185, 186, "")]
    public static extern bool ov_model_is_dynamic(ov_model* model);
    

    /// <summary>Do reshape in model with a list of &lt;name, partial shape&gt;.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_names">The list of input tensor names.</param>
    /// <param name="partial_shapes">A PartialShape list.</param>
    /// <param name="size">The item count in the list.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 197, 201, "ov_model_c_api")]
    public static extern ov_status_e ov_model_reshape(ov_model* model, byte** tensor_names, ov_partial_shape* partial_shapes, nint size);
    

    /// <summary>Do reshape in model with partial shape for a specified name.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="tensor_name">The tensor name of input tensor.</param>
    /// <param name="partial_shape">A PartialShape.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 211, 214, "ov_model_c_api")]
    public static extern ov_status_e ov_model_reshape_input_by_name(ov_model* model, byte* tensor_name, ov_partial_shape partial_shape);
    

    /// <summary>Do reshape in model for one node(port 0).</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="partial_shape">A PartialShape.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 223, 224, "ov_model_c_api")]
    public static extern ov_status_e ov_model_reshape_single_input(ov_model* model, ov_partial_shape partial_shape);
    

    /// <summary>Do reshape in model with a list of &lt;port id, partial shape&gt;.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="port_indexes">The array of port indexes.</param>
    /// <param name="partial_shape">A PartialShape list.</param>
    /// <param name="size">The item count in the list.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 235, 239, "ov_model_c_api")]
    public static extern ov_status_e ov_model_reshape_by_port_indexes(ov_model* model, nint* port_indexes, ov_partial_shape* partial_shape, nint size);
    

    /// <summary>Do reshape in model with a list of &lt;ov_output_port_t, partial shape&gt;.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="output_ports">The ov_output_port_t list.</param>
    /// <param name="partial_shapes">A PartialShape list.</param>
    /// <param name="size">The item count in the list.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 250, 254, "ov_model_c_api")]
    public static extern ov_status_e ov_model_reshape_by_ports(ov_model* model, ov_output_port** output_ports, ov_partial_shape* partial_shapes, nint size);
    

    /// <summary>Gets the friendly name for a model.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="friendly_name">the model&apos;s friendly name.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_model.h", 263, 264, "ov_model_c_api")]
    public static extern ov_status_e ov_model_get_friendly_name(ov_model* model, byte** friendly_name);
    

    /// <summary>Get the shape of port object.</summary>
    /// <param name="port">A pointer to ov_output_const_port_t.</param>
    /// <param name="tensor_shape">tensor shape.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 38, 39, "ov_node_c_api")]
    public static extern ov_status_e ov_const_port_get_shape(ov_output_const_port* port, ov_shape_t* tensor_shape);
    

    /// <summary>Get the shape of port object.</summary>
    /// <param name="port">A pointer to ov_output_port_t.</param>
    /// <param name="tensor_shape">tensor shape.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 48, 49, "ov_node_c_api")]
    public static extern ov_status_e ov_port_get_shape(ov_output_port* port, ov_shape_t* tensor_shape);
    

    /// <summary>Get the tensor name of port.</summary>
    /// <param name="port">A pointer to the ov_output_const_port_t.</param>
    /// <param name="tensor_name">A pointer to the tensor name.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 58, 59, "ov_node_c_api")]
    public static extern ov_status_e ov_port_get_any_name(ov_output_const_port* port, byte** tensor_name);
    

    /// <summary>Get the partial shape of port.</summary>
    /// <param name="port">A pointer to the ov_output_const_port_t.</param>
    /// <param name="partial_shape">Partial shape.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 68, 69, "ov_node_c_api")]
    public static extern ov_status_e ov_port_get_partial_shape(ov_output_const_port* port, ov_partial_shape* partial_shape);
    

    /// <summary>Get the tensor type of port.</summary>
    /// <param name="port">A pointer to the ov_output_const_port_t.</param>
    /// <param name="tensor_type">tensor type.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 78, 79, "ov_node_c_api")]
    public static extern ov_status_e ov_port_get_element_type(ov_output_const_port* port, ov_element_type_e* tensor_type);
    

    /// <summary>free port object</summary>
    /// <param name="port">The pointer to the instance of the ov_output_port_t to free.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 86, 87, "ov_node_c_api")]
    public static extern void ov_output_port_free(ov_output_port* port);
    

    /// <summary>free const port</summary>
    /// <param name="port">The pointer to the instance of the ov_output_const_port_t to free.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_node.h", 94, 95, "ov_node_c_api")]
    public static extern void ov_output_const_port_free(ov_output_const_port* port);
    

    /// <summary>Initialze a partial shape with static rank and dynamic dimension.</summary>
    /// <param name="rank">support static rank.</param>
    /// <param name="dims">
    /// <para>support dynamic and static dimension.</para>
    /// <para> Static rank, but dynamic dimensions on some or all axes.</para>
    /// <para>    Examples: `{1,2,?,4}` or `{?,?,?}` or `{1,2,-1,4}`</para>
    /// <para> Static rank, and static dimensions on all axes.</para>
    /// <para>    Examples: `{1,2,3,4}` or `{6}` or `{}`</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 49, 50, "ov_partial_shape_c_api")]
    public static extern ov_status_e ov_partial_shape_create(long rank, ov_dimension* dims, ov_partial_shape* partial_shape_obj);
    

    /// <summary>Initialze a partial shape with dynamic rank and dynamic dimension.</summary>
    /// <param name="rank">support dynamic and static rank.</param>
    /// <param name="dims">
    /// <para>support dynamic and static dimension.</para>
    /// <para> Dynamic rank:</para>
    /// <para>    Example: `?`</para>
    /// <para> Static rank, but dynamic dimensions on some or all axes.</para>
    /// <para>    Examples: `{1,2,?,4}` or `{?,?,?}` or `{1,2,-1,4}`</para>
    /// <para> Static rank, and static dimensions on all axes.</para>
    /// <para>    Examples: `{1,2,3,4}` or `{6}` or `{}&quot;`</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 66, 69, "ov_partial_shape_c_api")]
    public static extern ov_status_e ov_partial_shape_create_dynamic(ov_dimension rank, ov_dimension* dims, ov_partial_shape* partial_shape_obj);
    

    /// <summary>Initialize a partial shape with static rank and static dimension.</summary>
    /// <param name="rank">support static rank.</param>
    /// <param name="dims">
    /// <para>support static dimension.</para>
    /// <para> Static rank, and static dimensions on all axes.</para>
    /// <para>    Examples: `{1,2,3,4}` or `{6}` or `{}`</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 81, 82, "ov_partial_shape_c_api")]
    public static extern ov_status_e ov_partial_shape_create_static(long rank, long* dims, ov_partial_shape* partial_shape_obj);
    

    /// <summary>Release internal memory allocated in partial shape.</summary>
    /// <param name="partial_shape">The object&apos;s internal memory will be released.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 90, 91, "ov_partial_shape_c_api")]
    public static extern void ov_partial_shape_free(ov_partial_shape* partial_shape);
    

    /// <summary>Convert partial shape without dynamic data to a static shape.</summary>
    /// <param name="partial_shape">The partial_shape pointer.</param>
    /// <param name="shape">The shape pointer.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 100, 101, "ov_partial_shape_c_api")]
    public static extern ov_status_e ov_partial_shape_to_shape(ov_partial_shape partial_shape, ov_shape_t* shape);
    

    /// <summary>Convert shape to partial shape.</summary>
    /// <param name="shape">The shape pointer.</param>
    /// <param name="partial_shape">The partial_shape pointer.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 110, 111, "ov_partial_shape_c_api")]
    public static extern ov_status_e ov_shape_to_partial_shape(ov_shape_t shape, ov_partial_shape* partial_shape);
    

    /// <summary>Check this partial_shape whether is dynamic</summary>
    /// <param name="partial_shape">The partial_shape pointer.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 119, 120, "ov_partial_shape_c_api")]
    public static extern bool ov_partial_shape_is_dynamic(ov_partial_shape partial_shape);
    

    /// <summary>Helper function, convert a partial shape to readable string.</summary>
    /// <param name="partial_shape">The partial_shape pointer.</param>
    /// <returns>A string reprensts partial_shape&apos;s content.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_partial_shape.h", 128, 129, "ov_partial_shape_c_api")]
    public static extern byte* ov_partial_shape_to_string(ov_partial_shape partial_shape);
    

    /// <summary>Create a ov_preprocess_prepostprocessor_t instance.</summary>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 115, 116, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_create(ov_model* model, ov_preprocess_prepostprocessor** preprocess);
    

    /// <summary>Release the memory allocated by ov_preprocess_prepostprocessor_t.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 123, 124, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_prepostprocessor_free(ov_preprocess_prepostprocessor* preprocess);
    

    /// <summary>Get the input info of ov_preprocess_prepostprocessor_t instance.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 133, 135, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_input_info(ov_preprocess_prepostprocessor* preprocess, ov_preprocess_input_info** preprocess_input_info);
    

    /// <summary>Get the input info of ov_preprocess_prepostprocessor_t instance by tensor name.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="tensor_name">The name of input.</param>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 145, 148, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_input_info_by_name(ov_preprocess_prepostprocessor* preprocess, byte* tensor_name, ov_preprocess_input_info** preprocess_input_info);
    

    /// <summary>Get the input info of ov_preprocess_prepostprocessor_t instance by tensor order.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="tensor_index">The order of input.</param>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 158, 161, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_input_info_by_index(ov_preprocess_prepostprocessor* preprocess, nint tensor_index, ov_preprocess_input_info** preprocess_input_info);
    

    /// <summary>Release the memory allocated by ov_preprocess_input_info_t.</summary>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 168, 169, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_input_info_free(ov_preprocess_input_info* preprocess_input_info);
    

    /// <summary>Get a ov_preprocess_input_tensor_info_t.</summary>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <param name="preprocess_input_tensor_info">A pointer to ov_preprocess_input_tensor_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 178, 180, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_info_get_tensor_info(ov_preprocess_input_info* preprocess_input_info, ov_preprocess_input_tensor_info** preprocess_input_tensor_info);
    

    /// <summary>Release the memory allocated by ov_preprocess_input_tensor_info_t.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 187, 188, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_input_tensor_info_free(ov_preprocess_input_tensor_info* preprocess_input_tensor_info);
    

    /// <summary>Get a ov_preprocess_preprocess_steps_t.</summary>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <param name="preprocess_input_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 197, 199, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_info_get_preprocess_steps(ov_preprocess_input_info* preprocess_input_info, ov_preprocess_preprocess_steps** preprocess_input_steps);
    

    /// <summary>Release the memory allocated by ov_preprocess_preprocess_steps_t.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to the ov_preprocess_preprocess_steps_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 206, 207, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_preprocess_steps_free(ov_preprocess_preprocess_steps* preprocess_input_process_steps);
    

    /// <summary>Add resize operation to model&apos;s dimensions.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="resize_algorithm">A ov_preprocess_resizeAlgorithm instance</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 216, 218, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_resize(ov_preprocess_preprocess_steps* preprocess_input_process_steps, ov_preprocess_resize_algorithm_e resize_algorithm);
    

    /// <summary>Add scale preprocess operation. Divide each element of input by specified value.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="value">Scaling value</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 227, 228, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_scale(ov_preprocess_preprocess_steps* preprocess_input_process_steps, float value);
    

    /// <summary>Add scale preprocess operation. Divide each channel element of input by different specified value.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="values">Scaling values array for each channels</param>
    /// <param name="value_size">Scaling value size</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 238, 241, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_scale_multi_channels(ov_preprocess_preprocess_steps* preprocess_input_process_steps, float* values, int value_size);
    

    /// <summary>Add mean preprocess operation. Subtract specified value from each element of input.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="value">Value to subtract from each element.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 250, 251, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_mean(ov_preprocess_preprocess_steps* preprocess_input_process_steps, float value);
    

    /// <summary>Add mean preprocess operation. Subtract each channel element of input by different specified value.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="values">Value array to subtract from each element.</param>
    /// <param name="value_size">Mean value size</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 261, 264, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_mean_multi_channels(ov_preprocess_preprocess_steps* preprocess_input_process_steps, float* values, int value_size);
    

    /// <summary>Crop input tensor between begin and end coordinates.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="begin">
    /// <para>Pointer to begin indexes for input tensor cropping.</para>
    /// <para>Negative values represent counting elements from the end of input tensor</para>
    /// </param>
    /// <param name="begin_size">The size of begin array</param>
    /// <param name="end">
    /// <para>Pointer to end indexes for input tensor cropping.</para>
    /// <para>End indexes are exclusive, which means values including end edge are not included in the output slice.</para>
    /// <para>Negative values represent counting elements from the end of input tensor</para>
    /// </param>
    /// <param name="end_size">The size of end array</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 279, 284, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_crop(ov_preprocess_preprocess_steps* preprocess_input_process_steps, int* begin, int begin_size, int* end, int end_size);
    

    /// <summary>Add &apos;convert layout&apos; operation to specified layout.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <param name="layout">A point to ov_layout_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 293, 295, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_convert_layout(ov_preprocess_preprocess_steps* preprocess_input_process_steps, ov_layout* layout);
    

    /// <summary>Reverse channels operation.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to ov_preprocess_preprocess_steps_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 303, 304, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_reverse_channels(ov_preprocess_preprocess_steps* preprocess_input_process_steps);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t precesion.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="element_type">A point to element_type</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 313, 315, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_element_type(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_element_type_e element_type);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t color format.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="colorFormat">The enumerate of colorFormat</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 324, 326, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t color format with subname.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="colorFormat">The enumerate of colorFormat</param>
    /// <param name="sub_names_size">The size of sub_names</param>
    /// <remarks>variadic params sub_names Optional list of sub-names assigned for each plane (e.g. &quot;Y&quot;, &quot;UV&quot;).</remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 337, 342, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9);
    [DllImport(Dll, EntryPoint = nameof(ov_preprocess_input_tensor_info_set_color_format_with_subname), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_preprocess_input_tensor_info_set_color_format_with_subname(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_color_format_e colorFormat, nint sub_names_size, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t spatial_static_shape.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="input_height">The height of input</param>
    /// <param name="input_width">The width of input</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 352, 356, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_spatial_static_shape(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, nint input_height, nint input_width);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t memory type.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="mem_type">Memory type. Refer to ov_remote_context.h to get memory type string info.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 365, 367, "prepostprocess")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_memory_type(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, byte* mem_type);
    

    /// <summary>Convert ov_preprocess_preprocess_steps_t element type.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to the ov_preprocess_preprocess_steps_t.</param>
    /// <param name="element_type">preprocess input element type.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 376, 378, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_convert_element_type(ov_preprocess_preprocess_steps* preprocess_input_process_steps, ov_element_type_e element_type);
    

    /// <summary>Convert ov_preprocess_preprocess_steps_t color.</summary>
    /// <param name="preprocess_input_process_steps">A pointer to the ov_preprocess_preprocess_steps_t.</param>
    /// <param name="colorFormat">The enumerate of colorFormat.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 387, 389, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_convert_color(ov_preprocess_preprocess_steps* preprocess_input_process_steps, ov_color_format_e colorFormat);
    

    /// <summary>Helper function to reuse element type and shape from user&apos;s created tensor.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="tensor">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 398, 400, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_from(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_tensor* tensor);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t layout.</summary>
    /// <param name="preprocess_input_tensor_info">A pointer to the ov_preprocess_input_tensor_info_t.</param>
    /// <param name="layout">A point to ov_layout_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 409, 411, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_tensor_info_set_layout(ov_preprocess_input_tensor_info* preprocess_input_tensor_info, ov_layout* layout);
    

    /// <summary>Get the output info of ov_preprocess_output_info_t instance.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="preprocess_output_info">A pointer to the ov_preprocess_output_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 420, 422, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_output_info(ov_preprocess_prepostprocessor* preprocess, ov_preprocess_output_info** preprocess_output_info);
    

    /// <summary>Get the output info of ov_preprocess_output_info_t instance.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="tensor_index">The tensor index</param>
    /// <param name="preprocess_output_info">A pointer to the ov_preprocess_output_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 432, 435, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_output_info_by_index(ov_preprocess_prepostprocessor* preprocess, nint tensor_index, ov_preprocess_output_info** preprocess_output_info);
    

    /// <summary>Get the output info of ov_preprocess_output_info_t instance.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="tensor_name">The name of input.</param>
    /// <param name="preprocess_output_info">A pointer to the ov_preprocess_output_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 445, 448, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_get_output_info_by_name(ov_preprocess_prepostprocessor* preprocess, byte* tensor_name, ov_preprocess_output_info** preprocess_output_info);
    

    /// <summary>Release the memory allocated by ov_preprocess_output_info_t.</summary>
    /// <param name="preprocess_output_info">A pointer to the ov_preprocess_output_info_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 455, 456, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_output_info_free(ov_preprocess_output_info* preprocess_output_info);
    

    /// <summary>Get a ov_preprocess_input_tensor_info_t.</summary>
    /// <param name="preprocess_output_info">A pointer to the ov_preprocess_output_info_t.</param>
    /// <param name="preprocess_output_tensor_info">A pointer to the ov_preprocess_output_tensor_info_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 465, 467, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_output_info_get_tensor_info(ov_preprocess_output_info* preprocess_output_info, ov_preprocess_output_tensor_info** preprocess_output_tensor_info);
    

    /// <summary>Release the memory allocated by ov_preprocess_output_tensor_info_t.</summary>
    /// <param name="preprocess_output_tensor_info">A pointer to the ov_preprocess_output_tensor_info_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 474, 475, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_output_tensor_info_free(ov_preprocess_output_tensor_info* preprocess_output_tensor_info);
    

    /// <summary>Set ov_preprocess_input_tensor_info_t precesion.</summary>
    /// <param name="preprocess_output_tensor_info">A pointer to the ov_preprocess_output_tensor_info_t.</param>
    /// <param name="element_type">A point to element_type</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 484, 486, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_output_set_element_type(ov_preprocess_output_tensor_info* preprocess_output_tensor_info, ov_element_type_e element_type);
    

    /// <summary>Get current input model information.</summary>
    /// <param name="preprocess_input_info">A pointer to the ov_preprocess_input_info_t.</param>
    /// <param name="preprocess_input_model_info">A pointer to the ov_preprocess_input_model_info_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 495, 497, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_info_get_model_info(ov_preprocess_input_info* preprocess_input_info, ov_preprocess_input_model_info** preprocess_input_model_info);
    

    /// <summary>Release the memory allocated by ov_preprocess_input_model_info_t.</summary>
    /// <param name="preprocess_input_model_info">A pointer to the ov_preprocess_input_model_info_t to free memory.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 504, 505, "ov_prepostprocess_c_api")]
    public static extern void ov_preprocess_input_model_info_free(ov_preprocess_input_model_info* preprocess_input_model_info);
    

    /// <summary>Set layout for model&apos;s input tensor.</summary>
    /// <param name="preprocess_input_model_info">A pointer to the ov_preprocess_input_model_info_t</param>
    /// <param name="layout">A point to ov_layout_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 514, 516, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_input_model_info_set_layout(ov_preprocess_input_model_info* preprocess_input_model_info, ov_layout* layout);
    

    /// <summary>Adds pre/post-processing operations to function passed in constructor.</summary>
    /// <param name="preprocess">A pointer to the ov_preprocess_prepostprocessor_t.</param>
    /// <param name="model">A pointer to the ov_model_t.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 525, 526, "ov_prepostprocess_c_api")]
    public static extern ov_status_e ov_preprocess_prepostprocessor_build(ov_preprocess_prepostprocessor* preprocess, ov_model** model);
    

    /// <summary>Add pad preprocess operation. Extends an input tensor on edges with constants.</summary>
    /// <param name="preprocess_input_process_steps"> A pointer to the ov_preprocess_preprocess_steps_t.</param>
    /// <param name="pads_begin">                     Number of padding elements to add at the beginning of each axis.</param>
    /// <param name="pads_begin_size">                Pads begin size (number of axes).</param>
    /// <param name="pads_end">                       Number of padding elements to add at the end of each axis.</param>
    /// <param name="pads_end_size">                  Pads end size (number of axes).</param>
    /// <param name="value">                          Value to be populated in the padded area (mode=CONSTANT)</param>
    /// <param name="mode">                           Padding mode.</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_prepostprocess.h", 539, 546, "")]
    public static extern ov_status_e ov_preprocess_preprocess_steps_pad(ov_preprocess_preprocess_steps* preprocess_input_process_steps, int* pads_begin, nint pads_begin_size, int* pads_end, nint pads_end_size, float value, ov_padding_mode_e mode);
    

    /// <summary>Check this rank whether is dynamic</summary>
    /// <param name="rank">The rank pointer that will be checked.</param>
    /// <returns>bool The return value.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_rank.h", 28, 29, "ov_rank_c_api")]
    public static extern bool ov_rank_is_dynamic(ov_dimension rank);
    

    /// <summary>
    /// <para>Allocates memory tensor in device memory or wraps user-supplied memory handle</para>
    /// <para>using the specified tensor description and low-level device-specific parameters.</para>
    /// <para>Returns a pointer to the object that implements the RemoteTensor interface.</para>
    /// </summary>
    /// <param name="context">A pointer to the ov_remote_context_t instance.</param>
    /// <param name="type">Defines the element type of the tensor.</param>
    /// <param name="shape">Defines the shape of the tensor.</param>
    /// <param name="object_args_size">Size of the low-level tensor object parameters.</param>
    /// <param name="remote_tensor">Pointer to returned ov_tensor_t that contains remote tensor instance.</param>
    /// <remarks>variadic params Contains low-level tensor object parameters.</remarks>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 31, 37, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18);
    [DllImport(Dll, EntryPoint = nameof(ov_remote_context_create_tensor), CallingConvention = CallingConvention.Cdecl)] public static extern ov_status_e ov_remote_context_create_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, nint object_args_size, ov_tensor** remote_tensor, IntPtr varg1, IntPtr varg2, IntPtr varg3, IntPtr varg4, IntPtr varg5, IntPtr varg6, IntPtr varg7, IntPtr varg8, IntPtr varg9, IntPtr varg10, IntPtr varg11, IntPtr varg12, IntPtr varg13, IntPtr varg14, IntPtr varg15, IntPtr varg16, IntPtr varg17, IntPtr varg18, IntPtr varg19, IntPtr varg20);
    

    /// <summary>Returns name of a device on which underlying object is allocated.</summary>
    /// <param name="context">A pointer to the ov_remote_context_t instance.</param>
    /// <param name="device_name">Device name will be returned.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 46, 47, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_context_get_device_name(ov_remote_context* context, byte** device_name);
    

    /// <summary>
    /// <para>Returns a string contains device-specific parameters required for low-level</para>
    /// <para>operations with the underlying object.</para>
    /// <para>Parameters include device/context handles, access flags,</para>
    /// <para>etc. Content of the returned map depends on a remote execution context that is</para>
    /// <para>currently set on the device (working scenario).</para>
    /// <para>One actaul example: &quot;CONTEXT_TYPE OCL OCL_CONTEXT 0x5583b2ec7b40 OCL_QUEUE 0x5583b2e98ff0&quot;</para>
    /// </summary>
    /// <param name="context">A pointer to the ov_remote_context_t instance.</param>
    /// <param name="size">The size of param pairs.</param>
    /// <param name="params">Param name:value list.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 62, 63, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_context_get_params(ov_remote_context* context, nint* size, byte** @params);
    

    /// <summary>
    /// <para>This method is used to create a host tensor object friendly for the device in current context.</para>
    /// <para>For example, GPU context may allocate USM host memory (if corresponding extension is available),</para>
    /// <para>which could be more efficient than regular host memory.</para>
    /// </summary>
    /// <param name="context">A pointer to the ov_remote_context_t instance.</param>
    /// <param name="type">Defines the element type of the tensor.</param>
    /// <param name="shape">Defines the shape of the tensor.</param>
    /// <param name="tensor">Pointer to ov_tensor_t that contains host tensor.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 76, 80, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_context_create_host_tensor(ov_remote_context* context, ov_element_type_e type, ov_shape_t shape, ov_tensor** tensor);
    

    /// <summary>Release the memory allocated by ov_remote_context_t.</summary>
    /// <param name="context">A pointer to the ov_remote_context_t to free memory.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 88, 89, "ov_remote_context_c_api")]
    public static extern void ov_remote_context_free(ov_remote_context* context);
    

    /// <summary>
    /// <para>Returns a string contains device-specific parameters required for low-level</para>
    /// <para>operations with underlying object.</para>
    /// <para>Parameters include device/context/surface/buffer handles, access flags,</para>
    /// <para>etc. Content of the returned map depends on remote execution context that is</para>
    /// <para>currently set on the device (working scenario).</para>
    /// <para>One example: &quot;MEM_HANDLE:0x559ff6904b00;OCL_CONTEXT:0x559ff71d62f0;SHARED_MEM_TYPE:OCL_BUFFER;&quot;</para>
    /// </summary>
    /// <param name="tensor">Pointer to ov_tensor_t that contains host tensor.</param>
    /// <param name="size">The size of param pairs.</param>
    /// <param name="params">Param name:value list.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 104, 105, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_tensor_get_params(ov_tensor* tensor, nint* size, byte** @params);
    

    /// <summary>Returns name of a device on which underlying object is allocated.</summary>
    /// <param name="remote_tensor">A pointer to the remote tensor instance.</param>
    /// <param name="device_name">Device name will be return.</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_remote_context.h", 114, 115, "ov_remote_context_c_api")]
    public static extern ov_status_e ov_remote_tensor_get_device_name(ov_tensor* remote_tensor, byte** device_name);
    

    /// <summary>Initialize a fully shape object, allocate space for its dimensions and set its content id dims is not null.</summary>
    /// <param name="rank">The rank value for this object, it should be more than 0(&gt;0)</param>
    /// <param name="dims">The dimensions data for this shape object, it&apos;s size should be equal to rank.</param>
    /// <param name="shape">The input/output shape object pointer.</param>
    /// <returns>ov_status_e The return status code.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_shape.h", 33, 34, "ov_shape_c_api")]
    public static extern ov_status_e ov_shape_create(long rank, long* dims, ov_shape_t* shape);
    

    /// <summary>Free a shape object&apos;s internal memory.</summary>
    /// <param name="shape">The input shape object pointer.</param>
    /// <returns>ov_status_e The return status code.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_shape.h", 42, 43, "ov_shape_c_api")]
    public static extern ov_status_e ov_shape_free(ov_shape_t* shape);
    

    /// <summary>Constructs Tensor using element type, shape and external host ptr.</summary>
    /// <param name="type">Tensor element type</param>
    /// <param name="shape">Tensor shape</param>
    /// <param name="host_ptr">Pointer to pre-allocated host memory</param>
    /// <param name="tensor">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 33, 37, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_create_from_host_ptr(ov_element_type_e type, ov_shape_t shape, void* host_ptr, ov_tensor** tensor);
    

    /// <summary>Constructs Tensor using element type and shape. Allocate internal host storage using default allocator</summary>
    /// <param name="type">Tensor element type</param>
    /// <param name="shape">Tensor shape</param>
    /// <param name="tensor">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 47, 48, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_create(ov_element_type_e type, ov_shape_t shape, ov_tensor** tensor);
    

    /// <summary>Set new shape for tensor, deallocate/allocate if new total size is bigger than previous one.</summary>
    /// <param name="tensor">Tensor shape</param>
    /// <param name="shape">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 57, 58, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_set_shape(ov_tensor* tensor, ov_shape_t shape);
    

    /// <summary>Get shape for tensor.</summary>
    /// <param name="tensor">Tensor shape</param>
    /// <param name="shape">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 67, 68, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_get_shape(ov_tensor* tensor, ov_shape_t* shape);
    

    /// <summary>Get type for tensor.</summary>
    /// <param name="tensor">Tensor element type</param>
    /// <param name="type">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 77, 78, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_get_element_type(ov_tensor* tensor, ov_element_type_e* type);
    

    /// <summary>the total number of elements (a product of all the dims or 1 for scalar).</summary>
    /// <param name="tensor">number of elements</param>
    /// <param name="elements_size">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 87, 88, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_get_size(ov_tensor* tensor, nint* elements_size);
    

    /// <summary>the size of the current Tensor in bytes.</summary>
    /// <param name="tensor">the size of the current Tensor in bytes.</param>
    /// <param name="byte_size">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 97, 98, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_get_byte_size(ov_tensor* tensor, nint* byte_size);
    

    /// <summary>Provides an access to the underlaying host memory.</summary>
    /// <param name="tensor">A point to host memory.</param>
    /// <param name="data">A point to ov_tensor_t</param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 107, 108, "ov_tensor_c_api")]
    public static extern ov_status_e ov_tensor_data(ov_tensor* tensor, void** data);
    

    /// <summary>Free ov_tensor_t.</summary>
    /// <param name="tensor">A point to ov_tensor_t</param>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_tensor.h", 115, 116, "ov_tensor_c_api")]
    public static extern void ov_tensor_free(ov_tensor* tensor);
}
