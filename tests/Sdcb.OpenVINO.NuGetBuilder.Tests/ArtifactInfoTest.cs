using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class ArtifactInfoTest
{
    private readonly ITestOutputHelper _console;
    private readonly StorageNodeRoot _root;

    public ArtifactInfoTest(ITestOutputHelper console)
    {
        _console = console;
        string fileTreeJsonPath = @"asset/filetree.json";
        using FileStream stream = File.OpenRead(fileTreeJsonPath);
        _root = StorageNodeRoot.LoadRootFromStream(stream).GetAwaiter().GetResult();
    }

    [Fact]
    public void LatestStablePrintAllArtifacts()
    {
        foreach (ArtifactInfo artifact in _root.LatestStableVersion.Artifacts)
        {
            _console.WriteLine(artifact.ToString());
        }
    }

    [Fact]
    public void LatestPrintAllArtifacts()
    {
        foreach (ArtifactInfo artifact in _root.LatestVersion.Artifacts)
        {
            _console.WriteLine(artifact.ToString());
        }
    }
}
