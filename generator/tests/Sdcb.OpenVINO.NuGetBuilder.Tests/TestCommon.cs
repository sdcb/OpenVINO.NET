using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class TestCommon
{
    static TestCommon()
    {
        string fileTreeJsonPath = @"asset/filetree.json";
        using FileStream stream = File.OpenRead(fileTreeJsonPath);
        Root = StorageNodeRoot.LoadRootFromStream(stream).GetAwaiter().GetResult();
    }

    public const string WindowsKeysFile = @"./asset/openvino-windows-keys.txt";

    public static StorageNodeRoot Root { get; }
}
