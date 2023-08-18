using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

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
                .GroupBy(x => x.Name == "master")
                .SelectMany(x => x.Key switch
                {
                    true => x.First().EnumerateDirectories("").Select(x => VersionFolder.FromFolder(x)),
                    false => x.Select(x => VersionFolder.FromFolder(x))
                });
        }
    }

    public VersionFolder LatestStableVersion => VersionFolders
        .Where(x => !x.Version.IsPrerelease)
        .OrderByDescending(x => x.Version)
        .First();

    public VersionFolder LatestVersion => VersionFolders
        .OrderByDescending(x => x.Version)
        .First();
}
