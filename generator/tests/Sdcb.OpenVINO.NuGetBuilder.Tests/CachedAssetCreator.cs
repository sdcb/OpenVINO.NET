using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using SharpCompress.Archives;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class CachedAssetCreator
{
    [Fact(Skip = Reason)]
    public async Task CreateFileTree()
    {
        string url = $"{StorageNodeRoot.BaseUrl}/filetree.json";
        using HttpClient http = new();
        JsonDocument raw = await http.GetFromJsonAsync<JsonDocument>(url) ?? throw new Exception("Failed to load filetree from stream.");
        File.WriteAllBytes(Path.Combine(AssetFolder, "filetree.json"), JsonSerializer.SerializeToUtf8Bytes(raw, new JsonSerializerOptions 
        { 
            WriteIndented = true, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
        }));
    }

    [Theory(Skip = Reason)]
    [InlineData(KnownOS.Windows)]
    //[InlineData(KnownOS.MacOS)]
    //[InlineData(KnownOS.Linux)]
    public async Task CreateWindowsKeysTxt(KnownOS os)
    {
        StorageNodeRoot root = TestCommon.Root;
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == os);
        using HttpClient http = new ();
        byte[] zip = await http.GetByteArrayAsync(artifact.DownloadUrl);
        using IArchive archive = ArchiveFactory.Open(new MemoryStream(zip));
        File.WriteAllLines(Path.Combine(AssetFolder, $"openvino-{artifact.Distribution}-keys.txt"), archive.Entries.Select(x => x.Key));
    }

    static string AssetFolder { get; } = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.ToString(), "asset");

    const string Reason = "Too slow, only for developing time.";
}
