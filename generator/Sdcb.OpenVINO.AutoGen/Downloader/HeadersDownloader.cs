using NuGet.Versioning;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;

namespace Sdcb.OpenVINO.AutoGen.Downloader;

public class HeadersDownloader
{
    private readonly AppSettings _settings;
    private readonly StorageNodeRoot _root;
    private readonly ArtifactDownloader _downloader;

    public HeadersDownloader(AppSettings settings, StorageNodeRoot root, ArtifactDownloader downloader)
    {
        _settings = settings;
        _root = root;
        _downloader = downloader;
    }

    public async Task<ExtractedInfo> DownloadAsync(CancellationToken cancellationToken = default)
    {
        VersionFolder vf = _settings.Version switch
        {
            "LatestStable" => _root.LatestStableVersion,
            "Latest" => _root.LatestVersion,
            _ => _root.VersionFolders.Single(x => x.Version.ToString() == _settings.Version),
        };

        Directory.CreateDirectory(_settings.DownloadFolder);
        ArtifactInfo artifact = vf.Artifacts.Single(x => x.OS == KnownOS.Windows);

        return await _downloader.DownloadAndExtract(
            artifact,
            _settings.DownloadFolder,
            new WindowsHeadersLibFilter(),
            flatten: false,
            cancellationToken: cancellationToken);
    }

    public static async Task<ExtractedInfo> DirectDownloadAsync(
        string url, 
        ArtifactDownloader downloader,
        string downloadFolder,
        CancellationToken cancellationToken = default)
    {
        var artifact = new ArtifactInfo(KnownOS.Windows, "windows", "x64", SemanticVersion.Parse("2023.2.20231110"), new DateTime(2023, 11, 10), "zip", url, null);
        return await downloader.DownloadAndExtract(
            artifact,
            downloadFolder,
            new WindowsHeadersLibFilter(),
            flatten: false,
            cancellationToken: cancellationToken);
    }
}

internal class WindowsHeadersLibFilter : ILibFilter
{
    readonly WindowsLibFilter _w = new();

    public bool Filter(string key)
    {
        return key.EndsWith(".h") || _w.Filter(key);
    }
}