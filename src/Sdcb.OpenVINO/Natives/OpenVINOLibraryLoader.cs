using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.OpenVINO.Natives;

using static NativeMethods;

internal static class OpenVINOLibraryLoader
{
    internal static bool IsAndroid()
    {
        return Environment.OSVersion.Platform == PlatformID.Unix && Environment.GetEnvironmentVariable("ANDROID_ROOT") != null;
    }

    /// <summary>
    /// Initializes the OpenVINO library loader.
    /// </summary>
    public static void Init()
    {
        // stub to ensure static constructor executed at least once.
    }

    static OpenVINOLibraryLoader()
    {
#if LINQPad || NETCOREAPP3_1_OR_GREATER
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OpenVINOImportResolver);
#endif
    }

#if LINQPad || NETCOREAPP3_1_OR_GREATER

    private static readonly string[] SupportedVersionSuffixes = ["2620"];

    private static IntPtr OpenVINOImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == Dll)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadWithDeps(assembly, searchPath, new LibDeps("openvino_c.dll",
                [
                    "openvino.dll",
                ]));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadWithDeps(assembly, searchPath, SupportedVersionSuffixes.Select(v => new LibDeps($"libopenvino_c.{v}.dylib",
                [
                    $"libopenvino.{v}.dylib",
                ])).ToArray());
            }
            else if (IsAndroid())
            {
                return LoadWithDeps(assembly, searchPath, new LibDeps($"libopenvino_c.so",
                [
                    $"libopenvino.so",
                ]));
            }
            else
            {
                /* linux or others */
                return LoadWithDeps(assembly, searchPath, [.. SupportedVersionSuffixes.Select(v => new LibDeps($"libopenvino_c.so.{v}",
                [
                    $"libtbb.so.12",
                    $"libopenvino.so.{v}",
                ]))]);
            }
        }
        return IntPtr.Zero;
    }

    static IntPtr LoadWithDeps(Assembly assembly, DllImportSearchPath? searchPath, params LibDeps[] libDeps)
    {
        StringBuilder loadErrorMessage = new();

        foreach (LibDeps ld in libDeps)
        {
            Dictionary<string, IntPtr> loadStatus = ld.KnownDependencies
                .ToDictionary(k => k, knownDll =>
                {
                    NativeLibrary.TryLoad(knownDll, assembly, searchPath, out IntPtr handle);
                    return handle;
                });
            bool ok = NativeLibrary.TryLoad(ld.LibraryName, assembly, searchPath, out IntPtr handle);

            if (ok)
            {
                return handle;
            }
            else
            {
                loadErrorMessage.AppendLine($"Unable to load native library: {ld.LibraryName}, dependencies status: ");
                loadErrorMessage.AppendLine(String.Join("\n", loadStatus.Select(x => $"{x.Key}:{x.Value}")));
            }
        }

        throw new DllNotFoundException(loadErrorMessage.ToString());
    }
#endif
}

internal record LibDeps(string LibraryName, string[] KnownDependencies);
