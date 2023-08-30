using System;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;

[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_compiled_model.h", 26, 26)]
public struct ov_compiled_model
{
}


/// <summary>Represent all available devices.</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 63, 66)]
public unsafe struct ov_available_devices_t
{
    /// <summary>devices' name</summary>
    public byte** devices;
    
    /// <summary>devices' number</summary>
    public nint size;
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 26, 26)]
public struct ov_core
{
}


/// <summary>Represents version information that describes all devices and ov runtime library</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 53, 56)]
public unsafe struct ov_core_version_list_t
{
    /// <summary>An array of device versions</summary>
    public ov_core_version_t* versions;
    
    /// <summary>A number of versions in the array</summary>
    public nint size;
}


/// <summary>Represents version information that describes device and ov runtime library</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 43, 46)]
public unsafe struct ov_core_version_t
{
    /// <summary>A device name</summary>
    public byte* device_name;
    
    /// <summary>Version</summary>
    public ov_version version;
}


/// <summary>Represents OpenVINO version information</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_core.h", 33, 36)]
public unsafe struct ov_version
{
    /// <summary>A string representing OpenVINO version</summary>
    public byte* buildNumber;
    
    /// <summary>A string representing OpenVINO description</summary>
    public byte* description;
}


/// <summary>This is a structure interface equal to ov::Dimension</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_dimension.h", 20, 23)]
public struct ov_dimension
{
    /// <summary>The lower inclusive limit for the dimension.</summary>
    public long min;
    
    /// <summary>The upper inclusive limit for the dimension.</summary>
    public long max;
}


/// <summary>Completion callback definition about the function and args</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 29, 32)]
public unsafe struct ov_callback_t
{
    /// <summary>The callback func</summary>
    public delegate*<void*, void> callback_func;
    
    /// <summary>The args of callback func</summary>
    public void* args;
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 22, 22)]
public struct ov_infer_request
{
}


/// <summary>A list of profiling info data</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 57, 60)]
public unsafe struct ov_profiling_info_list_t
{
    /// <summary>The list of ov_profilling_info_t</summary>
    public ov_profiling_info_t* profiling_infos;
    
    /// <summary>The list size</summary>
    public nint size;
}


/// <summary>Store profiling info data</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_infer_request.h", 39, 50)]
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


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_layout.h", 20, 20)]
public struct ov_layout
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_model.h", 22, 22)]
public struct ov_model
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_node.h", 22, 22)]
public struct ov_output_const_port
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_node.h", 29, 29)]
public struct ov_output_port
{
}


/// <summary>It represents a shape that may be partially or totally dynamic. A PartialShape may have: Dynamic rank. (Informal notation: `?`) Static rank, but dynamic dimensions on some or all axes. (Informal notation examples: `{1,2,?,4}`, `{?,?,?}`, `{-1,-1,-1}`) Static rank, and static dimensions on all axes. (Informal notation examples: `{1,2,3,4}`, `{6}`, `{}`)</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_partial_shape.h", 32, 35)]
public unsafe struct ov_partial_shape
{
    /// <summary>The rank</summary>
    public ov_dimension rank;
    
    /// <summary>The dimension</summary>
    public ov_dimension* dims;
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 30, 30)]
public struct ov_preprocess_input_info
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 58, 58)]
public struct ov_preprocess_input_model_info
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 37, 37)]
public struct ov_preprocess_input_tensor_info
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 44, 44)]
public struct ov_preprocess_output_info
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 51, 51)]
public struct ov_preprocess_output_tensor_info
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 23, 23)]
public struct ov_preprocess_prepostprocessor
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_prepostprocess.h", 65, 65)]
public struct ov_preprocess_preprocess_steps
{
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_remote_context.h", 16, 16)]
public struct ov_remote_context
{
}


/// <summary>Reprents a static shape.</summary>
[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_shape.h", 20, 23)]
public unsafe struct ov_shape_t
{
    /// <summary>the rank of shape</summary>
    public long rank;
    
    /// <summary>the dims of shape</summary>
    public long* dims;
}


[StructLayout(LayoutKind.Sequential), CSourceInfo("ov_tensor.h", 22, 22)]
public struct ov_tensor
{
}

