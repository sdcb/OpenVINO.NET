using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;

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
            x => x.EndsWith(".h") || ArchiveExtractor.FilterWindowsDlls(x), 
            flatten: false, 
            cancellationToken: cancellationToken);
    }
}
