using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Security.Cryptography;

namespace Sdcb.OpenVINO.NuGetBuilder.Extractors;

public class WindowsSourceExtractor
{
    private readonly ICachedHttpGetService _http;

    public WindowsSourceExtractor(ICachedHttpGetService http)
    {
        _http = http;
    }

    public async Task<string> DownloadDynamicLibs(ArtifactInfo artifact, CancellationToken cancellationToken = default)
    {
        byte[] sha256 = await ReadSha256(artifact.Sha256Url, _http, cancellationToken);
        using Stream stream = await _http.DownloadAsStream(artifact.DownloadUrl, cancellationToken);
        VerifyStreamHash(artifact.DownloadUrl, sha256, stream);
        using IArchive archive = ArchiveFactory.Open(stream);
        IEnumerable<IArchiveEntry> dynamicLibs = archive.Entries.Where(x => FilterDynamicLibs(x.Key));

        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), artifact.Distribution);
        Directory.CreateDirectory(destinationFolder);
        Console.WriteLine($"Extracting dynamic libs into {destinationFolder}...");
        foreach (IArchiveEntry entry in dynamicLibs)
        {
            Console.WriteLine($"{entry.Key}...");
            entry.WriteToDirectory(artifact.Distribution, new ExtractionOptions
            {
                ExtractFullPath = false,
                Overwrite = true,
            });
        }
        return destinationFolder;
    }

    private static void VerifyStreamHash(string downloadUrl, byte[] sha256, Stream stream)
    {
        byte[] calculatedHash = SHA256.HashData(stream);
        if (!calculatedHash.SequenceEqual(sha256))
        {
            throw new Exception($"Calculated SHA256 {HexUtils.ByteArrayToHexString(calculatedHash)} mismatch for {downloadUrl}: {HexUtils.ByteArrayToHexString(sha256)}");
        }
        stream.Position = 0;
    }

    public static bool FilterDynamicLibs(string x)
    {
        return x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) &&
            !(x.Contains(@"/Debug/", StringComparison.OrdinalIgnoreCase) || x.Contains("_debug.", StringComparison.OrdinalIgnoreCase));
    }

    public static async Task<byte[]> ReadSha256(string sha256Url, ICachedHttpGetService http, CancellationToken cancellationToken = default)
    {
        using Stream stream = await http.DownloadAsStream(sha256Url, cancellationToken);
        using StreamReader reader = new(stream);
        string? line = await reader.ReadLineAsync(cancellationToken);
        string sha256String = line?.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0]
            ?? throw new Exception($"Failed to read sha256 from {sha256Url}");
        return HexUtils.HexStringToByteArray(sha256String);
    }
}
