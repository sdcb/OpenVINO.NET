namespace Sdcb.OpenVINO.Natives;

public static partial class NativeMethods
{
#if NET45_OR_GREATER
    public const string Dll = @"dll\win-x64\openvino_c.dll";
#else
    public const string Dll = "openvino_c";
#endif
}
