using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public class OpenVINOFileTree
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
    public List<OpenVINOFileTree>? Children { get; set; }

    public override string ToString() => $"{Type}: {Name}";

    private IEnumerable<OpenVINOFileTree> EnumerateItems(string targetPath, FileTreeType itemType, SearchOption searchOption)
    {
        if (Children == null) return Enumerable.Empty<OpenVINOFileTree>();
        targetPath = targetPath.Trim('/');

        if (targetPath == "")
        {
            return searchOption switch
            {
                SearchOption.TopDirectoryOnly => Children.Where(x => x.Type == itemType),
                SearchOption.AllDirectories => Children.Where(x => x.Type == itemType)
                                                   .Concat(Children.Where(x => x.Type == FileTreeType.Directory)
                                                   .SelectMany(x => x.EnumerateItems(targetPath, itemType, searchOption))),
                _ => throw new ArgumentOutOfRangeException(nameof(searchOption), searchOption, null),
            };
        }
        else
        {
            Dictionary<string, OpenVINOFileTree> children = Children.ToDictionary(k => k.Name, v => v);
            string[] segments = targetPath.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
            if (children.TryGetValue(segments[0], out OpenVINOFileTree? item))
            {
                return item.EnumerateItems(segments.Length > 1 ? segments[1] : "", itemType, searchOption);
            }
            else
            {
                throw new DirectoryNotFoundException($"Directory {targetPath} not found in {Name}.");
            }
        }
    }

    public IEnumerable<OpenVINOFileTree> EnumerateDirectories(string targetPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateItems(targetPath, FileTreeType.Directory, searchOption);
    }

    public IEnumerable<OpenVINOFileTree> EnumerateFiles(string targetPath, SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        return EnumerateItems(targetPath, FileTreeType.File, searchOption);
    }
}