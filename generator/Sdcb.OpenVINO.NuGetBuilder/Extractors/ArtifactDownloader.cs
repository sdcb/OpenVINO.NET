using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using SharpCompress.Archives;
using SharpCompress.Common;
using System.Security.Cryptography;

namespace Sdcb.OpenVINO.NuGetBuilder.Extractors;

public class ArtifactDownloader
{
    private readonly ICachedHttpGetService _http;

    public ArtifactDownloader(ICachedHttpGetService http)
    {
        _http = http;
    }

    public async Task<Stream> Download(ArtifactInfo artifact, CancellationToken cancellationToken = default)
    {
        byte[] sha256 = await ReadSha256(artifact.Sha256Url, _http, cancellationToken);
        Stream stream = await _http.DownloadAsStream(artifact.DownloadUrl, cancellationToken);
        VerifyStreamHash(artifact.DownloadUrl, sha256, stream);
        return stream;
    }

    public async Task<ExtractedInfo> DownloadAndExtract(ArtifactInfo artifact, string destinationFolder, Func<string, bool> keyFilter, bool flatten, CancellationToken cancellationToken = default)
    {
        using Stream stream = await Download(artifact, cancellationToken);
        return ArchiveExtractor.Extract(stream, destinationFolder, keyFilter, flatten);
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
