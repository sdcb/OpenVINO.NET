using ICSharpCode.SharpZipLib.Tar;
using System.IO.Compression;
using System.Text;

namespace Sdcb.OpenVINO.NuGetBuilders.Extractors;

public record ZipContainer(ZipEntrySlim[] Entries)
{
    public string RootFolderName => Entries.First().Key;

    public static ZipContainer Open(Stream stream)
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
        return new ZipContainer(entries);
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
