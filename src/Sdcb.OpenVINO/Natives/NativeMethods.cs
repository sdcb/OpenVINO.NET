namespace Sdcb.OpenVINO.Natives;

/// <summary>
/// Contains PInvoke methods to interact with the openvino_c.dll
/// </summary>
public static partial class NativeMethods
{
    /// <summary>
    /// Path to the openvino_c dynamic library
    /// </summary>
#if NET45_OR_GREATER
    public const string Dll = @"dll\win-x64\openvino_c.dll";
#else
    public const string Dll = "openvino_c";
#endif
}
