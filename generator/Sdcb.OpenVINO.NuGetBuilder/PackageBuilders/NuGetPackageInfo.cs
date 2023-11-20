using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilders.PackageBuilder;

public record NuGetPackageInfo(string NamePrefix, string Rid, SemanticVersion Version)
{
    public string Name => $"{NamePrefix}.runtime.{Rid}";

    public static NuGetPackageInfo FromArtifact(ArtifactInfo info)
    {
        string ridOs = info.Distribution switch
        {
            "centos7" => "centos.7",
            "debian9" => "linux", // debian9 on linux arm64 supports ubuntu 22.04, so should effectively equals to linux
            "ubuntu18" => "ubuntu.18.04",
            "ubuntu20" => "ubuntu.20.04",
            "ubuntu22" => "ubuntu.22.04",
            "macos_10_15" => "osx.10.15",
            "macos_11_0" => "osx.11.0",
            "windows" => "win",
            "rhel8" => "rhel.8",
            _ => throw new Exception($"Unknown distribution: {info.Distribution}")
        };
        string ridArch = info.Arch switch
        {
            "x86_64" => "x64",
            "arm64" => "arm64",
            "armhf" => "arm",
            _ => throw new Exception($"Unknown arch: {info.Arch}")
        };
        string rid = $"{ridOs}-{ridArch}";
        return new NuGetPackageInfo(GetNamePrefix(), rid, info.Version);
    }

    public static string GetNamePrefix() => $"{nameof(Sdcb)}.{nameof(OpenVINO)}";
}
