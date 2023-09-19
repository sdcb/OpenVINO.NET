using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using SharpCompress.Archives;
using SharpCompress.Common;

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
        IArchive archive = ArchiveFactory.Open(stream);
        _archives.Add(archive);
        foreach (IArchiveEntry node in archive.Entries)
        {
            if (node.Key.EndsWith(".tar"))
            {
                using MemoryStream ms = new();
                using Stream nodeStream = node.OpenEntryStream();
                nodeStream.CopyTo(ms);
                ms.Position = 0;
                ReadArchive(nodeStream);
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
                Console.WriteLine($"{entry.Key}...");
                entry.WriteToDirectory(destinationFolder, new ExtractionOptions
                {
                    ExtractFullPath = !flatten,
                    Overwrite = true,
                });
            }
        }

        return new ExtractedInfo(destinationFolder, reader.RootFolderName, localDlls);
    }
}
