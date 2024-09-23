using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;
using System.Text.Json;

namespace Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

public record StorageNodeRoot : StorageNode
{
    public const string BaseUrl = "https://storage.openvinotoolkit.org";

    public StorageNodeRoot(StorageNodeRaw raw, StorageNode? parent = null) : base(raw, parent)
    {
    }

    public static async Task<StorageNodeRoot> LoadRootFromHttp(IServiceProvider sp, CancellationToken cancellationToken = default)
    {
        ICachedHttpGetService http = sp.GetRequiredService<ICachedHttpGetService>();
        string url = $"{BaseUrl}/filetree.json";
        Stream stream = await http.DownloadAsStream(url, cancellationToken);
        return await LoadRootFromStream(stream, cancellationToken);
    }

    public static async Task<StorageNodeRoot> LoadRootFromStream(Stream stream, CancellationToken cancellationToken = default)
    {
        StorageNodeRaw rootNode = await JsonSerializer.DeserializeAsync<StorageNodeRaw>(stream, cancellationToken: cancellationToken) 
            ?? throw new Exception($"Failed to load filetree from stream.");

        return new StorageNodeRoot(rootNode, parent: null);
    }

    public IEnumerable<VersionFolder> VersionFolders
    {
        get
        {
            return EnumerateDirectories(VersionFolder.Prefix)
                .Where(x => x.Name != "master")
                .SelectMany(x => x.Name.StartsWith("202") switch
                {
                    true => [VersionFolder.FromFolder(x)], 
                    false => x.EnumerateDirectories("").Select(x => VersionFolder.FromFolder(x)),
                });
        }
    }

    public VersionFolder For(SemanticVersion version)
    {
        return VersionFolders
            .SingleOrDefault(x => x.Version == version) ?? throw new Exception($"Version {version} not found.");
    }

    public VersionFolder LatestStableVersion => VersionFolders
        .Where(x => !x.Version.IsPrerelease)
        .OrderByDescending(x => x.Version)
        .First();

    public VersionFolder LatestVersion => VersionFolders
        .OrderByDescending(x => x.Version)
        .First();
}
