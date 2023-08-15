using NuGet.Versioning;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;

public partial record VersionFolder(string Path, OpenVINOFileTree Folder, SemanticVersion Version)
{
    public const string Prefix = "/repositories/openvino/packages";

    public override string ToString() => Version.ToString();

    public static VersionFolder FromFolder(OpenVINOFileTree Folder)
    {
        SemanticVersion ver = ParseOpenVINOVersion(Folder.Name);
        string path = ver.IsPrerelease ? $"{Prefix}/master/{Folder.Name}" : $"{Prefix}/{Folder.Name}";
        return new VersionFolder(path, Folder, ver);
    }

    static SemanticVersion ParseOpenVINOVersion(string versionString)
    {
        string[] versionParts = versionString.Split('.');

        // If the version string is in the format 'major.minor', e.g., '2014.7'
        if (versionParts.Length == 2)
        {
            // Append '.0' to convert it into a valid Semantic Version format (major.minor.patch)
            versionString += ".0";
        }

        // If the version string is in the format 'major.minor.0.0.dev+time', e.g., '2022.7.0.0.dev20220714'
        else if (versionParts.Length == 4 && versionParts[3].StartsWith("dev"))
        {
            // Remove 'dev' part and convert the last part (time) into a valid build metadata part
            versionString = OpenVINODevVersionRegex().Replace(versionString, "$1-$2.$3");
        }

        // Try to parse the version string into a SemanticVersion object
        if (SemanticVersion.TryParse(versionString, out SemanticVersion? semVersion))
        {
            return semVersion;
        }
        else
        {
            throw new FormatException($"{versionString} is not a known OpenVINO Version");
        }
    }

    [GeneratedRegex("(.+)\\.(dev)(\\d{8})")]
    private static partial Regex OpenVINODevVersionRegex();

    public IEnumerable<ArtifactInfo> Artifacts => ArtifactInfo.FromFolder(this);
}