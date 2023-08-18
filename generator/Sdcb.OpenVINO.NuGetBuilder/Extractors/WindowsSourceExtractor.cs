using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using SharpCompress.Archives;
using SharpCompress.Writers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.NuGetBuilder.Extractors;

public class WindowsSourceExtractor
{
    private readonly ICachedHttpGetService _http;

    public WindowsSourceExtractor(ICachedHttpGetService http)
    {
        _http = http;
    }

    public async Task DownloadAsync(ArtifactInfo artifact, CancellationToken cancellationToken = default)
    {
        byte[] sha256 = await ReadSha256(artifact.Sha256Url, _http, cancellationToken);
        using Stream stream = await _http.DownloadAsStream(artifact.DownloadUrl, cancellationToken);
        byte[] calculatedHash = SHA256.HashData(stream);
        if (!calculatedHash.SequenceEqual(sha256))
        {
            throw new Exception($"Calculated SHA256 {ByteArrayToHexString(calculatedHash)} mismatch for {artifact.DownloadUrl}: {ByteArrayToHexString(sha256)}");
        }

        stream.Position = 0;
        IArchive archive = ArchiveFactory.Open(stream);
    }

    public static async Task<byte[]> ReadSha256(string sha256Url, ICachedHttpGetService http, CancellationToken cancellationToken = default)
    {
        using Stream stream = await http.DownloadAsStream(sha256Url, cancellationToken);
        using StreamReader reader = new(stream);
        string? line = await reader.ReadLineAsync(cancellationToken);
        string sha256String = line?.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)[0]
            ?? throw new Exception($"Failed to read sha256 from {sha256Url}");
        return HexStringToByteArray(sha256String);
    }

    internal static string ByteArrayToHexString(byte[] byteArray)
    {
        StringBuilder hex = new(byteArray.Length * 2);
        foreach (byte b in byteArray)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }

    internal static byte[] HexStringToByteArray(string hexString)
    {
        byte[] byteArray = new byte[hexString.Length / 2];
        for (int i = 0; i < byteArray.Length; i++)
        {
            string hex = hexString[(i * 2)..(i * 2 + 2)];
            byteArray[i] = byte.Parse(hex, NumberStyles.HexNumber);
        }
        return byteArray;
    }
}
