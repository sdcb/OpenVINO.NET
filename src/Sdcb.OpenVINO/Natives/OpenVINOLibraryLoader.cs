using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.OpenVINO.Natives;

using static NativeMethods;

internal static class OpenVINOLibraryLoader
{
    internal static void WindowsNetFXLoad()
    {
        if (!IsAndroid())
        {
            string libsPath = Path.GetDirectoryName(Process.GetCurrentProcess().Modules.Cast<ProcessModule>()
                .Single(x => Path.GetFileNameWithoutExtension(x.ModuleName) == "openvino_c")
                .FileName)!;
            AddLibPathToWindowsEnvironment(libsPath);
        }
    }

    internal static bool IsAndroid()
    {
        return Environment.OSVersion.Platform == PlatformID.Unix && Environment.GetEnvironmentVariable("ANDROID_ROOT") != null;
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

    internal static bool Is202302OrGreater() => VersionAbbr >= 2320;

    internal static int VersionAbbr;

#if LINQPad || NETCOREAPP3_1_OR_GREATER

    static OpenVINOLibraryLoader()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), OpenVINOImportResolver);
        VersionAbbr = OVCore.Version.GetAbbreviatedVersion();
    }

    private static IntPtr OpenVINOImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
    {
        if (libraryName == Dll)
        {
            string[] allowedVersions = new[] { "2320", "2310" };
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return LoadWithDeps(assembly, searchPath, new LibDeps("openvino_c.dll", new string[]
                {
                    "openvino.dll",
                }));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return LoadWithDeps(assembly, searchPath, allowedVersions.Select(v => new LibDeps($"libopenvino_c.{v}.dylib", new string[]
                {
                    $"libopenvino.{v}.dylib",
                })).ToArray());
            }
            else if (IsAndroid())
            {
                return LoadWithDeps(assembly, searchPath, new LibDeps($"libopenvino_c.so", new string[]
                {
                    $"libopenvino.so",
                }));
            }
            else
            {
                /* linux or others */
                return LoadWithDeps(assembly, searchPath, allowedVersions.Select(v => new LibDeps($"libopenvino_c.so.{v}", new string[]
                {
                    $"libtbb.so.12",
                    $"libopenvino.so.{v}",
                })).ToArray());
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