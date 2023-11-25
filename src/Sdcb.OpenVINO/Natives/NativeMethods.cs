using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("Sdcb.OpenVINO.Extensions.OpenCvSharp4")]
[assembly: InternalsVisibleTo("Sdcb.OpenVINO.Tests")]

namespace Sdcb.OpenVINO.Natives;

/// <summary>
/// Contains PInvoke methods to interact with the openvino_c.dll
/// </summary>
public static partial class NativeMethods
{
    static NativeMethods()
    {
#if LINQPAD || NETCOREAPP3_1_OR_GREATER
        OpenVINOLibraryLoader.Init();
#elif NETSTANDARD2_0_OR_GREATER || NET45_OR_GREATER
		OpenVINOLibraryLoader.WindowsNetFXLoad();
#endif
    }

    /// <summary>
    /// Path to the openvino_c dynamic library
    /// </summary>
#if NET45_OR_GREATER
    public const string Dll = @"dll\win-x64\openvino_c.dll";
#else
    public const string Dll = "openvino_c";
#endif
}
