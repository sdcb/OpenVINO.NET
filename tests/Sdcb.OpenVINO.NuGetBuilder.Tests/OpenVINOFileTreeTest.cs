using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using System.Text.Json;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class OpenVINOFileTreeTest
{
    private readonly ITestOutputHelper _console;
    private readonly OpenVINOFileTreeRoot _root;

    public OpenVINOFileTreeTest(ITestOutputHelper console)
    {
        _console = console;
        string fileTreeJsonPath = @"asset/filetree.json";
        _root = JsonSerializer.Deserialize<OpenVINOFileTreeRoot>(File.ReadAllText(fileTreeJsonPath)) ?? throw new Exception($"Failed to load {fileTreeJsonPath}.");
    }

    [Fact]
    public void ListVersions()
    {
        _console.WriteLine(string.Join(Environment.NewLine, _root.VersionFolders));
    }
}