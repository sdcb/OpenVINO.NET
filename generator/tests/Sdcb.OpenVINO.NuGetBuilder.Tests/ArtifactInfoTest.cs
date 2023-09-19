using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class ArtifactInfoTest
{
    private readonly ITestOutputHelper _console;

    public ArtifactInfoTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void LatestStablePrintAllArtifacts()
    {
        foreach (ArtifactInfo artifact in TestCommon.Root.LatestStableVersion.Artifacts)
        {
            _console.WriteLine(artifact.ToString());
        }
    }

    [Fact]
    public void LatestPrintAllArtifacts()
    {
        foreach (ArtifactInfo artifact in TestCommon.Root.LatestVersion.Artifacts)
        {
            _console.WriteLine(artifact.ToString());
        }
    }
}
