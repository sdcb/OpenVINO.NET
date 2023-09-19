using System.Text.Json.Serialization;

namespace Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

public record StorageNodeRaw
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("last modified")]
    public required string LastModified { get; set; }

    [JsonPropertyName("size")]
    public required long Size { get; set; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required FileTreeType Type { get; set; }

    [JsonPropertyName("children")]
    public List<StorageNodeRaw>? Children { get; set; }

    public override string ToString() => $"{Type}: {Name}";
}
