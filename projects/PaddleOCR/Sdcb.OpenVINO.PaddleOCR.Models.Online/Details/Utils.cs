using SharpCompress.Archives;
using SharpCompress.Archives.GZip;
using SharpCompress.Archives.Tar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;

internal static class Utils
{
    public static Task DownloadFile(Uri uri, string localFile, CancellationToken cancellationToken) => DownloadFiles(new Uri[] { uri }, localFile, cancellationToken);

    public static async Task DownloadFiles(Uri[] uris, string localFile, CancellationToken cancellationToken)
    {
        using HttpClient http = new();

        foreach (Uri uri in uris)
        {
            try
            {
                HttpResponseMessage resp = await http.GetAsync(uri, cancellationToken);
                if (!resp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to download: {uri}, status code: {(int)resp.StatusCode}({resp.StatusCode})");
                    continue;
                }

                using (FileStream file = File.OpenWrite(localFile))
                {
                    await resp.Content.CopyToAsync(file/*, cancellationToken*/);
                    return;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Failed to download: {uri}, {ex}");
                continue;
            }
            catch (TaskCanceledException)
            {
                Console.WriteLine($"Failed to download: {uri}, timeout.");
                continue;
            }
        }

        throw new Exception($"Failed to download {localFile} from all uris: {string.Join(", ", uris.Select(x => x.ToString()))}");
    }

    public static async Task DownloadAndExtractAsync(string name, Uri uri, string rootDir, CancellationToken cancellationToken, params string[] expectedFileNames)
    {
        Directory.CreateDirectory(rootDir);
        if (expectedFileNames == null || expectedFileNames.Length == 0)
        {
            expectedFileNames = new[] { "inference.pdiparams", "inference.pdmodel" };
        }

        if (!CheckLocalModel(rootDir, expectedFileNames, throwOnError: false))
        {
            string localTarFile = Path.Combine(rootDir, uri.Segments.Last());
            if (!File.Exists(localTarFile) || new FileInfo(localTarFile).Length == 0)
            {
                Console.WriteLine($"Downloading {name} model from {uri}");
                await DownloadFile(uri, localTarFile, cancellationToken);
            }

            Console.WriteLine($"Extracting {localTarFile} to {rootDir}");
            using (IArchive archive = ArchiveFactory.Open(localTarFile))
            {
                if (archive is GZipArchive)
                {
                    using Stream stream = archive.Entries.Single().OpenEntryStream();
                    using MemoryStream ms = new();
                    stream.CopyTo(ms);
                    ms.Position = 0;
                    using IArchive inner = ArchiveFactory.Open(ms);
                    if (inner is TarArchive innerTar)
                    {
                        ExtractTarArchiveToDirectory(innerTar, rootDir);
                    }
                    else
                    {
                        inner.WriteToDirectory(rootDir);
                    }
                }
                else if (archive is TarArchive tar)
                {
                    ExtractTarArchiveToDirectory(tar, rootDir);
                }
                else
                {
                    archive.WriteToDirectory(rootDir);
                }

                CheckLocalModel(rootDir, expectedFileNames, throwOnError: true);
            }

            File.Delete(localTarFile);
        }
    }

    private static void ExtractTarArchiveToDirectory(TarArchive archive, string rootDir)
    {
        List<IArchiveEntry> entries = archive.Entries
            .Where(x => !string.IsNullOrWhiteSpace(x.Key))
            .Cast<IArchiveEntry>()
            .ToList();
        string? commonTopLevelDirectory = GetCommonTopLevelDirectory(entries);

        foreach (IArchiveEntry entry in entries)
        {
            string relativePath = GetTarRelativePath(entry.Key!, commonTopLevelDirectory);
            if (string.IsNullOrEmpty(relativePath))
            {
                continue;
            }

            string destinationPath = GetDestinationPath(rootDir, relativePath);
            if (entry.IsDirectory)
            {
                Directory.CreateDirectory(destinationPath);
                continue;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
            using Stream entryStream = entry.OpenEntryStream();
            using FileStream fileStream = File.Create(destinationPath);
            entryStream.CopyTo(fileStream);
        }
    }

    private static string? GetCommonTopLevelDirectory(IEnumerable<IArchiveEntry> entries)
    {
        string? commonTopLevelDirectory = null;
        foreach (IArchiveEntry entry in entries.Where(x => !x.IsDirectory))
        {
            string[] segments = entry.Key!.Replace('\\', '/').Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length <= 1)
            {
                return null;
            }

            if (commonTopLevelDirectory == null)
            {
                commonTopLevelDirectory = segments[0];
            }
            else if (!string.Equals(commonTopLevelDirectory, segments[0], StringComparison.Ordinal))
            {
                return null;
            }
        }

        return commonTopLevelDirectory;
    }

    private static string GetTarRelativePath(string entryKey, string? commonTopLevelDirectory)
    {
        string normalizedPath = entryKey.Replace('\\', '/').Trim('/');
        if (commonTopLevelDirectory == null)
        {
            return normalizedPath;
        }

        string prefix = commonTopLevelDirectory + "/";
        if (string.Equals(normalizedPath, commonTopLevelDirectory, StringComparison.Ordinal))
        {
            return string.Empty;
        }

        if (normalizedPath.StartsWith(prefix, StringComparison.Ordinal))
        {
            return normalizedPath.Substring(prefix.Length);
        }

        return normalizedPath;
    }

    private static string GetDestinationPath(string rootDir, string relativePath)
    {
        string destinationPath = Path.GetFullPath(Path.Combine(rootDir, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        string normalizedRootDir = Path.GetFullPath(rootDir)
            .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        if (!destinationPath.StartsWith(normalizedRootDir, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Archive entry extracts outside target directory: {relativePath}");
        }

        return destinationPath;
    }

    public static void CheckLocalOCRModel(string rootDir)
    {
        CheckLocalModel(rootDir, new[] { "inference.pdiparams", "inference.pdmodel" }, throwOnError: true);
    }

    private static bool CheckLocalModel(string rootDir, string[] expectedFileNames, bool throwOnError)
    {
        foreach (string fileName in expectedFileNames)
        {
            string path = Path.Combine(rootDir, fileName);

            if (!File.Exists(path))
            {
                if (!throwOnError) return false;
                throw new Exception($"{fileName} not found in {rootDir}, model error?");
            }

            if (new FileInfo(path).Length == 0)
            {
                if (!throwOnError) return false;
                throw new Exception($"{fileName} invalid(length = 0), model error?");
            }
        }

        return true;
    }

    public readonly static Type RootType = typeof(Settings);
    public readonly static Assembly RootAssembly = typeof(Settings).Assembly;
}
