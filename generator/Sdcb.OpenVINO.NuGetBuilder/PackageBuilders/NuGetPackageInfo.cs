using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilders.PackageBuilder;

public record NuGetPackageInfo(string NamePrefix, string Rid, string TitleRid, SemanticVersion Version)
{
    public string Name => $"{NamePrefix}.runtime.{TitleRid}";

    public static NuGetPackageInfo FromArtifact(ArtifactInfo info)
    {
        string titleRidOS = info.Distribution switch
        {
            "centos7" => "centos.7",
            "debian9" => "linux", // debian9 on linux arm64 supports ubuntu 22.04, so should effectively equals to linux
            "debian10" => "linux", // debian9 on linux arm64 supports ubuntu 22.04, so should effectively equals to linux
            "ubuntu18" => "ubuntu.18.04",
            "ubuntu20" => "ubuntu.20.04",
            "ubuntu22" => "ubuntu.22.04",
            "ubuntu24" => "ubuntu.24.04",
            "macos_10_15" => "osx.10.15",
            "macos_11_0" => "osx.11.0",
            "windows" => "win",
            "windows_vc_mt" => "win-mt",
            "rhel8" => "rhel.8",
            _ => throw new Exception($"Unknown distribution: {info.Distribution}")
        };
        string ridOS = info.Distribution switch
        {
            "centos7" => "linux",
            "debian9" => "linux", 
            "debian10" => "linux",
            "ubuntu18" => "linux",
            "ubuntu20" => "linux",
            "ubuntu22" => "linux",
            "ubuntu24" => "linux",
            "macos_10_15" => "osx",
            "macos_11_0" => "osx",
            "windows" => "win",
            "windows_vc_mt" => "win", // This is a special case for Windows artifacts with MT (Multi-threaded) runtime
            "rhel8" => "linux",
            _ => throw new Exception($"Unknown distribution: {info.Distribution}")
        };
        string ridArch = info.Arch switch
        {
            "x86_64" => "x64",
            "arm64" => "arm64",
            "armhf" => "arm",
            _ => throw new Exception($"Unknown arch: {info.Arch}")
        };
        string rid = $"{ridOS}-{ridArch}";
        string titleRid = $"{titleRidOS}-{ridArch}";
        return new NuGetPackageInfo(GetNamePrefix(), rid, titleRid, info.Version);
    }

    public static string GetNamePrefix() => $"{nameof(Sdcb)}.{nameof(OpenVINO)}";
}
