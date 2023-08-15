using Microsoft.Extensions.DependencyInjection;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public class OpenVINOFileTreeRoot : OpenVINOFileTree
{
    public const string BaseUrl = "https://storage.openvinotoolkit.org";

    public static async Task<OpenVINOFileTreeRoot> LoadRootAsync(IServiceProvider sp, CancellationToken cancellationToken = default)
    {
        CachedHttpGetService http = sp.GetRequiredService<CachedHttpGetService>();
        string url = $"{BaseUrl}/filetree.json";
        return await http.DownloadAsJsonAsync<OpenVINOFileTreeRoot>(url, cancellationToken);
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
