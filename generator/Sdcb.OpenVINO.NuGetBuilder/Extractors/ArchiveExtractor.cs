using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilder.Utils;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;

namespace Sdcb.OpenVINO.NuGetBuilders.Extractors;

internal class ArchiveReader : IDisposable
{
    private readonly List<IArchive> _archives = new();
    private readonly List<IArchiveEntry> _entries = new();

    public static ArchiveReader Open(Stream stream)
    {
        ArchiveReader reader = new();
        reader.ReadArchive(stream);
        return reader;
    }

    public IReadOnlyList<IArchiveEntry> Entries => _entries.ToArray();

    public string RootFolderName => _entries.First().Key.Split('/', StringSplitOptions.RemoveEmptyEntries).First();

    public void Dispose()
    {
        foreach (var archive in _archives)
        {
            archive.Dispose();
        }
    }

    private void ReadArchive(Stream stream)
    {
        IArchive archive = ArchiveFactory.Open(stream, new ReaderOptions { LookForHeader = true });
        _archives.Add(archive);
        foreach (IArchiveEntry node in archive.Entries)
        {
            if (node.Key == null  || node.Key.EndsWith(".tar"))
            {
                MemoryStream ms = new();
                using Stream nodeStream = node.OpenEntryStream();
                nodeStream.CopyTo(ms);
                ms.Position = 0;
                ReadArchive(ms);
            }
            else
            {
                _entries.Add(node);
            }
        }
    }
}

public class ArchiveExtractor
{
    public static ExtractedInfo Extract(Stream stream, string destinationFolder, ILibFilter keyFilter, bool flatten)
    {
        using Stream _ = stream;
        using ArchiveReader reader = ArchiveReader.Open(stream);
        IArchiveEntry[] dynamicLibs = reader.Entries
            .Where(x => keyFilter.Filter(x.Key))
            .ToArray();
        HashSet<string> knownPrefixes = dynamicLibs.Select(x => PathUtils.GetPart1FileNameWithoutExtension(x.Key)).ToHashSet();
        Dictionary<string, IArchiveEntry[]> sameFileNameCaches = reader.Entries
            .Select(x => (key: PathUtils.GetPart1FileNameWithoutExtension(x.Key), value: x))
            .Where(x => knownPrefixes.Contains(x.key))
            .GroupBy(x => x.key)
            .ToDictionary(k => k.Key, v => v.Select(x => x.value).OrderBy(x => x.Key.Length).ToArray());

        Directory.CreateDirectory(destinationFolder);

        string[] localDlls = dynamicLibs
            .Select(x => Path.Combine(destinationFolder, flatten ? Path.GetFileName(x.Key) : x.Key))
            .ToArray();
        if (localDlls.All(File.Exists))
        {
            Console.WriteLine($"Extracted artifacts already exists, skip.");
        }
        else
        {
            Console.WriteLine($"Extracting artifacts into {destinationFolder}...");
            foreach (IArchiveEntry entry in dynamicLibs)
            {
                if (entry.Size > 0)
                {
                    Console.WriteLine($"{entry.Key}...");
                    entry.WriteToDirectory(destinationFolder, new ExtractionOptions
                    {
                        ExtractFullPath = !flatten,
                        Overwrite = true,
                    });
                }
                else
                {
                    // seems like a link
                    IArchiveEntry[] sameFileNameEntries = sameFileNameCaches[PathUtils.GetPart1FileNameWithoutExtension(entry.Key)];
                    int startIndex = Array.FindIndex(sameFileNameEntries, x => x.Key == entry.Key);
                    bool copied = false;
                    for (int i = startIndex + 1; i < sameFileNameEntries.Length; i++)
                    {
                        IArchiveEntry linkedEntry = sameFileNameEntries[i];
                        if (linkedEntry.Size > 0)
                        {
                            Console.WriteLine($"{entry.Key} -> {Path.GetFileName(linkedEntry.Key)}");
                            using Stream linkedStream = linkedEntry.OpenEntryStream();
                            string destFolder = flatten ? destinationFolder : Path.Combine(destinationFolder, Path.GetDirectoryName(entry.Key)!);
                            Directory.CreateDirectory(destFolder);
                            string destFile = Path.Combine(destinationFolder, Path.GetFileName(entry.Key));
                            using FileStream file = File.OpenWrite(destFile);
                            linkedStream.CopyTo(file);
                            copied = true;
                            break;
                        }
                    }

                    if (!copied)
                    {
                        throw new Exception($"Entry failed to find link: {entry.Key}.");
                    }
                }
            }
        }

        return new ExtractedInfo(destinationFolder, reader.RootFolderName, localDlls);
    }
}
