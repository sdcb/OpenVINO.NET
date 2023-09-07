#pragma warning disable CS1591
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;



/// <summary>type define ov_compiled_model_t from ov_compiled_model</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_compiled_model.h", 26, 26, "ov_compiled_model_c_api")]
public struct ov_compiled_model
{
}


/// <summary>type define ov_core_t from ov_core</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 26, 26, "ov_core_c_api")]
public struct ov_core
{
}


/// <summary>Represents OpenVINO version information</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 33, 36, "ov_core_c_api")]
public unsafe struct ov_version
{
    /// <summary>A string representing OpenVINO version</summary>
    public byte* buildNumber;

    /// <summary>A string representing OpenVINO description</summary>
    public byte* description;
}


/// <summary> Represents version information that describes device and ov runtime library</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 43, 46, "ov_core_c_api")]
public unsafe struct ov_core_version_t
{
    /// <summary>A device name</summary>
    public byte* device_name;

    /// <summary>Version</summary>
    public ov_version version;
}


/// <summary> Represents version information that describes all devices and ov runtime library</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 53, 56, "ov_core_c_api")]
public unsafe struct ov_core_version_list_t
{
    /// <summary>An array of device versions</summary>
    public ov_core_version_t* versions;

    /// <summary>A number of versions in the array</summary>
    public nint size;
}


/// <summary>Represent all available devices.</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 63, 66, "ov_core_c_api")]
public unsafe struct ov_available_devices_t
{
    /// <summary>devices&apos; name</summary>
    public byte** devices;

    /// <summary>devices&apos; number</summary>
    public nint size;
}


/// <summary>This is a structure interface equal to ov::Dimension</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_dimension.h", 20, 23, "ov_dimension_c_api")]
public struct ov_dimension
{
    /// <summary>The lower inclusive limit for the dimension.</summary>
    public long min;

    /// <summary>The upper inclusive limit for the dimension.</summary>
    public long max;
}


/// <summary>type define ov_infer_request_t from ov_infer_request</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 22, 22, "ov_infer_request_c_api")]
public struct ov_infer_request
{
}


/// <summary>Completion callback definition about the function and args</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 29, 32, "ov_infer_request_c_api")]
public unsafe struct ov_callback_t
{
    /// <summary>The callback func</summary>
    public delegate*<void*, void> callback_func;

    /// <summary>The args of callback func</summary>
    public void* args;
}


/// <summary>Store profiling info data</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 39, 50, "ov_infer_request_c_api")]
public unsafe struct ov_profiling_info_t
{
    /// <summary>status</summary>
    public Status status;

    /// <summary>The absolute time, in microseconds, that the node ran (in total).</summary>
    public long real_time;

    /// <summary>The net host CPU time that the node ran.</summary>
    public long cpu_time;

    /// <summary>Name of a node.</summary>
    public byte* node_name;

    /// <summary>Execution type of a unit.</summary>
    public byte* exec_type;

    /// <summary>Node type.</summary>
    public byte* node_type;
}


/// <summary>A list of profiling info data</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 57, 60, "ov_infer_request_c_api")]
public unsafe struct ov_profiling_info_list_t
{
    /// <summary>The list of ov_profilling_info_t</summary>
    public ov_profiling_info_t* profiling_infos;

    /// <summary>The list size</summary>
    public nint size;
}


/// <summary>type define ov_layout_t from ov_layout</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_layout.h", 20, 20, "ov_layout_c_api")]
public struct ov_layout
{
}


/// <summary>type define ov_model_t from ov_model</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_model.h", 22, 22, "ov_model_c_api")]
public struct ov_model
{
}


/// <summary>type define ov_output_const_port_t from ov_output_const_port</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_node.h", 22, 22, "ov_node_c_api")]
public struct ov_output_const_port
{
}


/// <summary>type define ov_output_port_t from ov_output_port</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_node.h", 29, 29, "ov_node_c_api")]
public struct ov_output_port
{
}


/// <summary>
/// <para>It represents a shape that may be partially or totally dynamic.</para>
/// <para>A PartialShape may have:</para>
/// <para>Dynamic rank. (Informal notation: `?`)</para>
/// <para>Static rank, but dynamic dimensions on some or all axes.</para>
/// <para>    (Informal notation examples: `{1,2,?,4}`, `{?,?,?}`, `{-1,-1,-1}`)</para>
/// <para>Static rank, and static dimensions on all axes.</para>
/// <para>    (Informal notation examples: `{1,2,3,4}`, `{6}`, `{}`)</para>
/// <para>An interface to make user can initialize ov_partial_shape_t</para>
/// </summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_partial_shape.h", 32, 35, "ov_partial_shape_c_api")]
public unsafe struct ov_partial_shape
{
    /// <summary>The rank</summary>
    public ov_dimension rank;

    /// <summary>The dimension</summary>
    public ov_dimension* dims;
}


/// <summary>type define ov_preprocess_prepostprocessor_t from ov_preprocess_prepostprocessor</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 23, 23, "ov_prepostprocess_c_api")]
public struct ov_preprocess_prepostprocessor
{
}


/// <summary>type define ov_preprocess_input_info_t from ov_preprocess_input_info</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 30, 30, "ov_prepostprocess_c_api")]
public struct ov_preprocess_input_info
{
}


/// <summary>type define ov_preprocess_input_tensor_info_t from ov_preprocess_input_tensor_info</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 37, 37, "ov_prepostprocess_c_api")]
public struct ov_preprocess_input_tensor_info
{
}


/// <summary>type define ov_preprocess_output_info_t from ov_preprocess_output_info</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 44, 44, "ov_prepostprocess_c_api")]
public struct ov_preprocess_output_info
{
}


/// <summary>type define ov_preprocess_output_tensor_info_t from ov_preprocess_output_tensor_info</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 51, 51, "ov_prepostprocess_c_api")]
public struct ov_preprocess_output_tensor_info
{
}


/// <summary>type define ov_preprocess_input_model_info_t from ov_preprocess_input_model_info</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 58, 58, "ov_prepostprocess_c_api")]
public struct ov_preprocess_input_model_info
{
}


/// <summary>type define ov_preprocess_preprocess_steps_t from ov_preprocess_preprocess_steps</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 65, 65, "ov_prepostprocess_c_api")]
public struct ov_preprocess_preprocess_steps
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_remote_context.h", 16, 16, "")]
public struct ov_remote_context
{
}


/// <summary>Reprents a static shape.</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_shape.h", 20, 23, "ov_shape_c_api")]
public unsafe struct ov_shape_t
{
    /// <summary>the rank of shape</summary>
    public long rank;

    /// <summary>the dims of shape</summary>
    public long* dims;
}


/// <summary>type define ov_tensor_t from ov_tensor</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_tensor.h", 22, 22, "ov_tensor_c_api")]
public struct ov_tensor
{
}
