using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
#if LINQPAD || NET6_0_OR_GREATER
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

    /// <summary>
    /// Retrieves the last error message and resets the error state.
    /// </summary>
    /// <returns>A pointer to the last error message.</returns>
    /// <remarks>
    /// The returned pointer is valid until the next call to any OpenVINO function.
    /// </remarks>
    /// <seealso cref="ov_get_error_info(ov_status_e)"/>
    [DllImport(Dll, CallingConvention = CallingConvention.Cdecl)]
    public static unsafe extern byte* ov_get_and_reset_last_error();
}
