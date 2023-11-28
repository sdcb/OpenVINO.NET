using NuGet.Versioning;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders.PackageBuilder;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class NuGetPackageInfoTests
{
    [Fact]
    public void Debian9IsEffectivelyLinuxOnArm()
    {
        ArtifactInfo ai = new(KnownOS.Linux, "debian9", "arm64", SemanticVersion.Parse("2023.2.0"), DateTime.Now, "zip", "", null);
        NuGetPackageInfo ni = NuGetPackageInfo.FromArtifact(ai);

        Assert.Equal("linux-arm64", ni.TitleRid);
    }
}
