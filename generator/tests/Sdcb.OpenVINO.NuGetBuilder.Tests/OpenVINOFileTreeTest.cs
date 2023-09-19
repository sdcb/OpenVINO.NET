using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class OpenVINOFileTreeTest
{
    private readonly ITestOutputHelper _console;

    public OpenVINOFileTreeTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    public void ListVersions()
    {
        _console.WriteLine(string.Join(Environment.NewLine, TestCommon.Root.VersionFolders.OrderByDescending(x => x.Version)));
    }

    [Fact]
    public void PrintLatestVersion()
    {
        _console.WriteLine(TestCommon.Root.LatestStableVersion.ToString());
    }

    [Fact]
    public void ListPaths()
    {
        _console.WriteLine(string.Join(Environment.NewLine, TestCommon.Root.VersionFolders.OrderByDescending(x => x.Version).Select(x => x.Path)));
    }

    [Fact]
    public void ListContainsInLatestStableVersion()
    {
        VersionFolder vf = TestCommon.Root.LatestStableVersion;
        _console.WriteLine(string.Join(Environment.NewLine, vf.Folder.EnumerateFiles("").Select(x => x.Name)));
    }
}