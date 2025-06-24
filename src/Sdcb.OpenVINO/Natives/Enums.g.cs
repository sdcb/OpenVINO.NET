#pragma warning disable CS1591
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;



/// <summary>This enum contains codes for all possible return values of the interface functions</summary>
[CSourceInfo("ov_common.h", 135, 163, "ov_base_c_api")]
public enum ov_status_e
{
    /// <summary>SUCCESS</summary>
    OK = 0,

    /// <summary>GENERAL_ERROR</summary>
    GENERAL_ERROR = -1,

    /// <summary>NOT_IMPLEMENTED</summary>
    NOT_IMPLEMENTED = -2,

    /// <summary>NETWORK_NOT_LOADED</summary>
    NETWORK_NOT_LOADED = -3,

    /// <summary>PARAMETER_MISMATCH</summary>
    PARAMETER_MISMATCH = -4,

    /// <summary>NOT_FOUND</summary>
    NOT_FOUND = -5,

    /// <summary>OUT_OF_BOUNDS</summary>
    OUT_OF_BOUNDS = -6,

    /// <summary>UNEXPECTED</summary>
    UNEXPECTED = -7,

    /// <summary>REQUEST_BUSY</summary>
    REQUEST_BUSY = -8,

    /// <summary>RESULT_NOT_READY</summary>
    RESULT_NOT_READY = -9,

    /// <summary>NOT_ALLOCATED</summary>
    NOT_ALLOCATED = -10,

    /// <summary>INFER_NOT_STARTED</summary>
    INFER_NOT_STARTED = -11,

    /// <summary>NETWORK_NOT_READ</summary>
    NETWORK_NOT_READ = -12,

    /// <summary>INFER_CANCELLED</summary>
    INFER_CANCELLED = -13,

    /// <summary>INVALID_C_PARAM</summary>
    INVALID_C_PARAM = -14,

    /// <summary>UNKNOWN_C_ERROR</summary>
    UNKNOWN_C_ERROR = -15,

    /// <summary>NOT_IMPLEMENT_C_METHOD</summary>
    NOT_IMPLEMENT_C_METHOD = -16,

    /// <summary>UNKNOW_EXCEPTION</summary>
    UNKNOW_EXCEPTION = -17,
}


/// <summary>
/// <para>This enum contains codes for element type, which is aligned with ov::element::Type_t in</para>
/// <para>src/core/include/openvino/core/type/element_type.hpp</para>
/// </summary>
[CSourceInfo("ov_common.h", 171, 199, "ov_base_c_api")]
public enum ov_element_type_e
{
    /// <summary>Undefined element type</summary>
    UNDEFINED = 0,

    /// <summary>Dynamic element type</summary>
    DYNAMIC = 0,

    OV_BOOLEAN = 1,

    /// <summary>bf16 element type</summary>
    BF16 = 2,

    /// <summary>f16 element type</summary>
    F16 = 3,

    /// <summary>f32 element type</summary>
    F32 = 4,

    /// <summary>f64 element type</summary>
    F64 = 5,

    /// <summary>i4 element type</summary>
    I4 = 6,

    /// <summary>i8 element type</summary>
    I8 = 7,

    /// <summary>i16 element type</summary>
    I16 = 8,

    /// <summary>i32 element type</summary>
    I32 = 9,

    /// <summary>i64 element type</summary>
    I64 = 10,

    /// <summary>binary element type</summary>
    U1 = 11,

    /// <summary>u2 element type</summary>
    U2 = 12,

    /// <summary>u3 element type</summary>
    U3 = 13,

    /// <summary>u4 element type</summary>
    U4 = 14,

    /// <summary>u6 element type</summary>
    U6 = 15,

    /// <summary>u8 element type</summary>
    U8 = 16,

    /// <summary>u16 element type</summary>
    U16 = 17,

    /// <summary>u32 element type</summary>
    U32 = 18,

    /// <summary>u64 element type</summary>
    U64 = 19,

    /// <summary>nf4 element type</summary>
    NF4 = 20,

    /// <summary>f8e4m3 element type</summary>
    F8E4M3 = 21,

    /// <summary>f8e5m2 element type</summary>
    F8E5M3 = 22,

    /// <summary>string element type</summary>
    STRING = 23,

    /// <summary>f4e2m1 element type</summary>
    F4E2M1 = 24,

    /// <summary>f8e8m0 element type</summary>
    F8E8M0 = 25,
}


[CSourceInfo("ov_infer_request.h", 40, 44, "")]
public enum Status
{
    /// <summary>A node is not executed.</summary>
    NOT_RUN = 0,

    /// <summary>A node is optimized out during graph optimization phase.</summary>
    OPTIMIZED_OUT = 1,

    /// <summary>A node is executed.</summary>
    EXECUTED = 2,
}


/// <summary>This enum contains enumerations for color format.</summary>
[CSourceInfo("ov_prepostprocess.h", 72, 83, "ov_prepostprocess_c_api")]
public enum ov_color_format_e
{
    /// <summary>Undefine color format</summary>
    UNDEFINE = 0,

    /// <summary>Image in NV12 format as single tensor</summary>
    NV12_SINGLE_PLANE = 1,

    /// <summary>Image in NV12 format represented as separate tensors for Y and UV planes.</summary>
    NV12_TWO_PLANES = 2,

    /// <summary>Image in I420 (YUV) format as single tensor</summary>
    I420_SINGLE_PLANE = 3,

    /// <summary>Image in I420 format represented as separate tensors for Y, U and V planes.</summary>
    I420_THREE_PLANES = 4,

    /// <summary>Image in RGB interleaved format (3 channels)</summary>
    RGB = 5,

    /// <summary>Image in BGR interleaved format (3 channels)</summary>
    BGR = 6,

    /// <summary>Image in GRAY format (1 channel)</summary>
    GRAY = 7,

    /// <summary>Image in RGBX interleaved format (4 channels)</summary>
    RGBX = 8,

    /// <summary>Image in BGRX interleaved format (4 channels)</summary>
    BGRX = 9,
}


/// <summary>This enum contains codes for all preprocess resize algorithm.</summary>
[CSourceInfo("ov_prepostprocess.h", 90, 94, "ov_prepostprocess_c_api")]
public enum ov_preprocess_resize_algorithm_e
{
    /// <summary>linear algorithm</summary>
    RESIZE_LINEAR = 0,

    /// <summary>cubic algorithm</summary>
    RESIZE_CUBIC = 1,

    /// <summary>nearest algorithm</summary>
    RESIZE_NEAREST = 2,
}


/// <summary>This enum contains enumeration for  padding mode.</summary>
[CSourceInfo("ov_prepostprocess.h", 101, 106, "ov_prepostprocess_c_api")]
public enum ov_padding_mode_e
{
    /// <summary>Pads with given constant value.</summary>
    CONSTANT = 0,

    /// <summary>Pads with tensor edge values.</summary>
    EDGE = 1,

    /// <summary>Pads with reflection of tensor data along axis. Values on the edges are not duplicated.</summary>
    REFLECT = 2,

    /// <summary> Pads similar like `REFLECT` but values on the edges are duplicated.</summary>
    SYMMETRIC = 3,
}
