using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;

public static unsafe partial class NativeMethods
{
    /// <summary>Reads models from IR / ONNX / PDPD / TF / TFLite formats.</summary>
    /// <param name="core">A pointer to the ie_core_t instance.</param>
    /// <param name="model_str">String with a model in IR / ONNX / PDPD / TF / TFLite format, string is null-terminated.</param>
    /// <param name="weights">Shared pointer to a constant tensor with weights.</param>
    /// <param name="model">
    /// <para>A pointer to the newly created model.</para>
    /// <para>Reading ONNX / PDPD / TF / TFLite models does not support loading weights from the @p weights tensors.</para>
    /// </param>
    /// <returns>Status code of the operation: OK(0) for success.</returns>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo("ov_core.h", 188, 193, "ov_core_c_api")]
    public static extern ov_status_e ov_core_read_model_from_memory(ov_core* core, byte* model_str, ov_tensor* weights, ov_model** model);
}
