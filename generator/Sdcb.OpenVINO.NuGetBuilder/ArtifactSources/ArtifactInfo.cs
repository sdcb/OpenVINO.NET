using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public record ArtifactInfo(string OS, string Arch, SemanticVersion Version, string DownloadUrl, string Sha256)
{
}
