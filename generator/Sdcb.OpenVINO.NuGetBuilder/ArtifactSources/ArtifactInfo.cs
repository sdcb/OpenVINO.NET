﻿using NuGet.Versioning;
using System.Text.RegularExpressions;

namespace Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

public partial record ArtifactInfo(KnownOS OS, string Distribution, string Arch, SemanticVersion Version, DateTime UpdateTime, string ArchiveType, string DownloadUrl, string? Sha256Url)
{
    public static ArtifactInfo FromPath(StorageNode sha256Node, SemanticVersion providedVersion)
    {
        string fileName = ArtifactFileExtractor(sha256Node.Name);
        StorageNode? fileNode = sha256Node.Parent!.Children!.FirstOrDefault(x => x.Name == fileName)
            ?? throw new Exception($".sha256 node detected without corresponding file node for: {sha256Node.FullPath}");

        Match match = OpenVINOArtifactNameRegex().Match(fileNode.Name);
        if (!match.Success)
        {
            throw new FormatException($"{fileNode.Name} failed to detected as {nameof(ArtifactInfo)}.");
        }

        KnownOS os = ParseKnownOSChar(match.Groups["dist"].Value);

        return new ArtifactInfo(os, match.Groups["dist"].Value, match.Groups["arch"].Value, providedVersion, sha256Node.LastModified, match.Groups["ext"].Value,
            fileNode.FullPath, sha256Node.FullPath);
    }

    private static KnownOS ParseKnownOSChar(string dist)
    {
        return dist switch
        {
            "debian10" or "rhel8" or "ubuntu22" or "centos7" or "ubuntu20" or "ubuntu24" => KnownOS.Linux,
            "macos_12_6" => KnownOS.MacOS,
            "windows" => KnownOS.Windows,
            "windows_vc_mt" => KnownOS.Windows, // This is a special case for Windows artifacts with MT (Multi-threaded) runtime
            _ => throw new FormatException($"Failed to parse {dist} as {nameof(KnownOS)}.")
        };
    }

    internal static bool NameIdentifier(string name) => name.EndsWith(HashSuffix) && !name.StartsWith("pdb_");
    internal static string ArtifactFileExtractor(string name) => name[..(^HashSuffix.Length)];
    private const string HashSuffix = ".sha256";

    [GeneratedRegex(@"^openvino_toolkit_(?<dist>\w+)_(?<version>\d{4}\.\d\.\d\.[\w\.]+)_(?<arch>armhf|x86_64|arm64)\.(?<ext>\w+)$")]
    private static partial Regex OpenVINOArtifactNameRegex();

    internal static IEnumerable<ArtifactInfo> FromFolder(VersionFolder vf)
    {
        return vf.Folder.EnumerateFiles("", SearchOption.AllDirectories)
            .Where(x => NameIdentifier(x.Name))
            .Select(x => FromPath(x, vf.Version));
    }
}

public enum KnownOS
{
    Linux, 
    MacOS, 
    Windows
}