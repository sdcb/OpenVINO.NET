using NuGet.Versioning;
using System.IO;
using System.Text.RegularExpressions;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public partial record ArtifactInfo(KnownOS OS, string Distribution, string Arch, SemanticVersion Version, string DownloadUrl, string Sha256Url)
{
    public static ArtifactInfo FromPath(string folderPath, SemanticVersion providedVersion, string path)
    {
        string downloadUrl = $"{OpenVINOFileTreeRoot.BaseUrl}{folderPath}/{path}";
        string sha256Url = $"{downloadUrl}{HashSuffix}";
        Match match = OpenVINOArtifactNameRegex().Match(path);
        if (!match.Success) throw new FormatException($"{path} failed to detected as {nameof(ArtifactInfo)}.");

        KnownOS os = osFolder.Name switch
        {
            "linux" => KnownOS.Linux,
            "macos" => KnownOS.MacOS,
            "windows" => KnownOS.Windows,
            _ => throw new FormatException($"Failed to parse {osFolder.Name} as {nameof(KnownOS)}.")
        };

        return new ArtifactInfo(os, match.Groups["dist"].Value, match.Groups["arch"].Value, providedVersion, downloadUrl, sha256Url);
    }

    internal static bool NameIdentifier(string name) => name.EndsWith(HashSuffix);
    internal static string ArtifactFileExtractor(string name) => name[..(^HashSuffix.Length)];
    private const string HashSuffix = ".sha256";

    [GeneratedRegex("^(?<os>[m|w|l])_openvino_toolkit_(?<dist>\\w+)_(?<version>\\d{4}\\.\\d\\.\\d\\.[\\w\\.]+)_(?<arch>armhf|x86_64|arm64)\\.(?<ext>\\w+)$")]
    private static partial Regex OpenVINOArtifactNameRegex();

    internal static IEnumerable<ArtifactInfo> FromFolder(VersionFolder vf)
    {
        return vf.Folder.EnumerateFiles("", SearchOption.AllDirectories)
            .Where(x => NameIdentifier(x.Name))
            .Select(x => FromPath($"{vf.Path}/{osFolder.Name}", vf.Version, ArtifactFileExtractor(x.Name)));


    }
}

public enum KnownOS
{
    Linux, 
    MacOS, 
    Windows
}