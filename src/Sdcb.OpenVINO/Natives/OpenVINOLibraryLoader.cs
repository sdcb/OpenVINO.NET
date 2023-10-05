using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO.Natives;

using static NativeMethods;

internal static class OpenVINOLibraryLoader
{
    internal static void WindowsNetFXLoad()
    {
        _ = OVCore.Version;
        string libsPath = Path.GetDirectoryName(Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
            .Single(x => Path.GetFileNameWithoutExtension(x.ModuleName) == "openvino_c")
            .FileName)!;
        AddLibPathToWindowsEnvironment(libsPath);
    }

    internal static void AddLibPathToWindowsEnvironment(string libPath)
    {
        const string envId = "PATH";
        Environment.SetEnvironmentVariable(envId, Environment.GetEnvironmentVariable(envId) + Path.PathSeparator + libPath);
    }

    /// <summary>
    /// Initializes the OpenVINO library loader.
    /// </summary>
    public static void Init()
    {
        // stub to ensure static constructor executed at least once.
    }

#if LINQPad || NETCOREAPP3_1_OR_GREATER

    static OpenVINOLibraryLoader()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OpenVINOImportResolver);
    }

    private static IntPtr OpenVINOImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == Dll)
        {
            const string ver = "2310";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadWithDeps("openvino_c.dll", assembly, searchPath, new string[]
                {
                    "openvino.dll",
                });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadWithDeps($"libopenvino_c.{ver}.dylib", assembly, searchPath, new string[]
                {
                    $"libopenvino.{ver}.dylib",
                });
            }
            else
            {
                /* linux or others */
                return LoadWithDeps($"libopenvino_c.so.{ver}", assembly, searchPath, new string[]
                {
                    $"libopenvino.so.{ver}",
                });
            }
        }
        return IntPtr.Zero;
    }

    static IntPtr LoadWithDeps(string libraryName, Assembly assembly, DllImportSearchPath? searchPath, string[] knownDependencies)
    {
        string dependenciesLoadStatus = string.Join(Environment.NewLine, knownDependencies
            .Select(knownDll =>
            {
                bool loadOk = NativeLibrary.TryLoad(knownDll, assembly, searchPath, out IntPtr handle);
                string loadStatus = loadOk ? "OK" : "FAIL";
                return $"{knownDll}: {loadStatus}, handle={handle:x}";
            }));

        try
        {
            return NativeLibrary.Load(libraryName, assembly, searchPath);
        }
        catch (DllNotFoundException ex)
        {
            throw new DllNotFoundException(
                $"Unable to load shared library '{libraryName}', dependencies load status:{Environment.NewLine}{dependenciesLoadStatus}", ex);
        }
    }
#endif
}
