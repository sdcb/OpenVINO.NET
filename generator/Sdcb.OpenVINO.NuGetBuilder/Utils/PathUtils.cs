using System.Diagnostics.CodeAnalysis;

namespace Sdcb.OpenVINO.NuGetBuilder.Utils;

internal class PathUtils
{
    [return: NotNullIfNotNull(nameof(path))]
    public static string? GetPart1FileNameWithoutExtension(string? path)
    {
        if (path == null)
        {
            return null;
        }

        ReadOnlySpan<char> fileNameWithoutExtension = GetPart1FileNameWithoutExtension(path.AsSpan());
        if (path!.Length == fileNameWithoutExtension.Length)
        {
            return path;
        }
        return fileNameWithoutExtension.ToString();
    }

    public static ReadOnlySpan<char> GetPart1FileNameWithoutExtension(ReadOnlySpan<char> path)
    {
        ReadOnlySpan<char> fileName = Path.GetFileName(path);
        int num = fileName.IndexOf('.');
        if (num >= 0)
        {
            return fileName[..num];
        }
        return fileName;
    }
}
