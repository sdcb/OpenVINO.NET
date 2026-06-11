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

    [Fact]
    public void Centos8IsMappedToLinuxRid()
    {
        ArtifactInfo ai = new(KnownOS.Linux, "centos8", "x86_64", SemanticVersion.Parse("2026.2.0"), DateTime.Now, "tgz", "", null);
        NuGetPackageInfo ni = NuGetPackageInfo.FromArtifact(ai);

        Assert.Equal("linux-x64", ni.Rid);
        Assert.Equal("centos.8-x64", ni.TitleRid);
    }

    [Fact]
    public void AndroidIsMappedToAndroidRid()
    {
        ArtifactInfo ai = new(KnownOS.Android, "android", "x86_64", SemanticVersion.Parse("2026.2.0"), DateTime.Now, "tgz", "", null);
        NuGetPackageInfo ni = NuGetPackageInfo.FromArtifact(ai);

        Assert.Equal("android-x64", ni.Rid);
        Assert.Equal("android-x64", ni.TitleRid);
    }
}
