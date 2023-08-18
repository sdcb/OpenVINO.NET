using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

namespace Sdcb.OpenVINO.NuGetBuilder.Tests;

public class TestCommon
{
    static TestCommon()
    {
        string fileTreeJsonPath = @"asset/filetree.json";
        using FileStream stream = File.OpenRead(fileTreeJsonPath);
        Root = StorageNodeRoot.LoadRootFromStream(stream).GetAwaiter().GetResult();
    }

    public static StorageNodeRoot Root { get; }
}
