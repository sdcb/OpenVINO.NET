using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;

public partial record VersionFolder(string Path, StorageNode Folder, SemanticVersion Version)
{
    public const string Prefix = "/repositories/openvino/packages";

    public override string ToString() => Version.ToString();

    public static VersionFolder FromFolder(StorageNode Folder)
    {
        SemanticVersion ver = ParseOpenVINOVersion(Folder.Name);
        string path = ver.IsPrerelease ? $"{Prefix}/master/{Folder.Name}" : $"{Prefix}/{Folder.Name}";
        return new VersionFolder(path, Folder, ver);
    }

    /// <summary>
    /// Supported formats:
    /// <list type="bullet">
    /// <item>2024.3.0-15549-ae01c973d53</item>
    /// <item>2022.1</item>
    /// <item>2022.3.1</item>
    /// <item>2024.2.0rc1</item>
    /// </list>
    /// </summary>
    /// <exception cref="ArgumentException"></exception>
    public static SemanticVersion ParseOpenVINOVersion(string versionString)
    {
        // Split into major, minor, patch and optional additional label parts
        string[] parts = versionString.Split('.');

        if (parts.Length < 2)
        {
            throw new ArgumentException("Invalid version string");
        }

        // Parse the major and minor versions
        int major = int.Parse(parts[0]);
        int minor = int.Parse(parts[1]);

        // Initialize patch and label
        int patch = 0;
        string? releaseLabel = null;

        if (parts.Length > 2)
        {
            // Possible formats can be "patch", "patch-...", "patchrc..."
            // Split third part further by '-' to separate patch and prerelease label if exists
            string[] subParts = parts[2].Split(['-'], 2);

            // Parse patch version which is always the first part
            // Handle cases where the patch part might have a release candidate or similar suffix directly attached
            string patchPart = subParts[0];
            int releaseLabelIndex = FindFirstNonNumericIndex(patchPart);

            if (releaseLabelIndex != -1)
            {
                // Separate the numeric patch from the non-numeric release label
                patch = int.Parse(patchPart[..releaseLabelIndex]);
                releaseLabel = patchPart[releaseLabelIndex..];
            }
            else
            {
                patch = int.Parse(patchPart);
            }

            // If there is any further information after a '-', it is part of the release label
            if (subParts.Length > 1)
            {
                releaseLabel = (releaseLabel != null ? releaseLabel + "-" : "") + subParts[1];
            }
        }

        // Create the SemanticVersion based on extracted information
        if (releaseLabel != null)
        {
            return new SemanticVersion(major, minor, patch, releaseLabel);
        }
        else
        {
            return new SemanticVersion(major, minor, patch);
        }

        static int FindFirstNonNumericIndex(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (!char.IsDigit(str[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }

    public IEnumerable<ArtifactInfo> Artifacts => ArtifactInfo.FromFolder(this);
}