using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using System.Net.Http.Json;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Sdcb.OpenVINO.NuGetBuilders.Tests;

public class CachedAssetCreator
{
    [Fact]
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

    static string AssetFolder { get; } = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).Parent!.Parent!.Parent!.ToString(), "asset");

    const string Reason = "Too slow, only for developing time.";
}
