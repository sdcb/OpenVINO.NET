using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class ArtifactInfoTest
{
    private readonly ITestOutputHelper _console;
    private readonly OpenVINOFileTreeRoot _root;

    public ArtifactInfoTest(ITestOutputHelper console)
    {
        _console = console;
        string fileTreeJsonPath = @"asset/filetree.json";
        _root = JsonSerializer.Deserialize<OpenVINOFileTreeRoot>(File.ReadAllText(fileTreeJsonPath)) ?? throw new Exception($"Failed to load {fileTreeJsonPath}.");
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
