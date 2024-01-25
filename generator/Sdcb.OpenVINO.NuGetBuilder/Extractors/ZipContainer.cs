using ICSharpCode.SharpZipLib.Tar;
using System.IO.Compression;
using System.Text;

namespace Sdcb.OpenVINO.NuGetBuilders.Extractors;

public record ZipContainer(string ZipType, ZipEntrySlim[] Entries)
{
    public string RootFolderName => ZipType switch
    {
        "gz" => Entries[0].Key,
        "zip" => Entries[0].Key[..Entries[0].Key.IndexOf('/')],
        _ => throw new Exception($"Unknown archive type: {ZipType}")
    };

    public static ZipContainer Open(Stream stream)
    {
        string fileType = CheckArchiveType(stream);
        if (fileType == "gz")
        {
            using GZipStream gzip = new(stream, CompressionMode.Decompress);
            using TarInputStream tarInput = new(gzip, Encoding.UTF8);
            ZipEntrySlim[] entries = ReadEntries(tarInput).ToArray();

            Dictionary<string, ZipEntrySlim> cache = entries.ToDictionary(k => k.Key, v => v);
            foreach (ZipEntrySlim entry in entries)
            {
                if (entry.LinkName != null)
                {
                    if (cache.TryGetValue(entry.LinkName, out ZipEntrySlim? target))
                    {
                        entry.LinkTarget = target;
                    }
                }
            }
            return new ZipContainer(fileType, entries);
        }
        else if (fileType == "zip")
        {
            using ZipArchive zipArchive = new(stream, ZipArchiveMode.Read);
            return new ZipContainer(fileType, zipArchive.Entries
                .Select(x =>
                {
                    using MemoryStream ms = new();
                    using Stream zipStream = x.Open();
                    zipStream.CopyTo(ms);
                    return new ZipEntrySlim(x.FullName, ms.ToArray(), null);
                })
                .ToArray());
        }
        else
        {
            throw new Exception($"Unknown archive type: {fileType}");
        }
    }

    static string CheckArchiveType(Stream stream)
    {
        // 需要确保在读取之后，能够恢复stream的原始位置
        long originalPosition = stream.Position;
        byte[] buffer = new byte[4]; // 读取最长的magic number长度
        try
        {
            // 读取前几个字节
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            if (bytesRead < 2) // 至少需要两个字节来识别gzip
            {
                return "Unknown";
            }

            // 恢复stream的原始位置
            stream.Position = originalPosition;

            // 检查gzip的magic number(1F 8B)
            if (buffer[0] == 0x1F && buffer[1] == 0x8B)
            {
                return "gz";
            }

            // 检查zip的magic number(50 4B 03 04, 50 4B 05 06, 或 50 4B 07 08)
            if (buffer[0] == 0x50 && buffer[1] == 0x4B &&
               (buffer[2] == 0x03 && buffer[3] == 0x04 ||
                buffer[2] == 0x05 && buffer[3] == 0x06 ||
                buffer[2] == 0x07 && buffer[3] == 0x08))
            {
                return "zip";
            }

            return "Unknown";
        }
        catch
        {
            // 在处理stream时可能会有异常发生
            return "Error";
        }
    }

    static IEnumerable<ZipEntrySlim> ReadEntries(TarInputStream tarInput)
    {
        while (true)
        {
            TarEntry? tarEntry = tarInput.GetNextEntry();
            if (tarEntry == null) break;

            byte[] data = new byte[tarEntry.Size];
            using MemoryStream ms = new(data);
            tarInput.CopyEntryContents(ms);
            yield return new ZipEntrySlim(tarEntry.Name, data, tarEntry.TarHeader.LinkName != "" ?
                tarEntry.Name[..tarEntry.Name.LastIndexOf('/')] + '/' + tarEntry.TarHeader.LinkName : null);
        }
    }
}

public record ZipEntrySlim(string Key, byte[] Data, string? LinkName)
{
    public ZipEntrySlim? LinkTarget { get; set; }

    public ZipEntrySlim? GetLinkedRoot()
    {
        if (LinkTarget is null) return null;
        if (LinkTarget.LinkTarget is null) return LinkTarget;
        return LinkTarget.GetLinkedRoot();
    }
}
