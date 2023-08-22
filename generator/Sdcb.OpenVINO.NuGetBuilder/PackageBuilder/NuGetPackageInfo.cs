using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilder.PackageBuilder;

public record NuGetPackageInfo(string NamePrefix, string Rid, SemanticVersion Version)
{
    public string Name => $"{NamePrefix}.runtime.{Rid}";

    public static NuGetPackageInfo FromArtifact(ArtifactInfo info)
    {
        string ridOs = info.Distribution switch
        {
            "centos7" => "centos",
            "debian9" => "debian",
            "ubuntu18" => "ubuntu.18.04",
            "ubuntu20" => "ubuntu.20.04",
            "ubuntu22" => "ubuntu",
            "macos_10_15" => "osx.10.15",
            "macos_11_0" => "osx.11.0",
            "windows" => "win",
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
        string namePrefix = $"{nameof(Sdcb)}.{nameof(OpenVINO)}";
        return new NuGetPackageInfo(namePrefix, rid, info.Version);
    }
}
