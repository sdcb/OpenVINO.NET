using SharpCompress.Archives;
using SharpCompress.Common;

namespace Sdcb.OpenVINO.NuGetBuilder.Extractors;

public class ArchiveExtractor
{
    public static ExtractedInfo Extract(Stream stream, string destinationFolder, Func<string, bool> keyFilter, bool flatten)
    {
        using Stream _ = stream;
        using IArchive archive = ArchiveFactory.Open(stream);
        IArchiveEntry[] dynamicLibs = archive.Entries
            .Where(x => keyFilter(x.Key))
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

        string rootFolderName = archive.Entries.First().Key.Split('/', StringSplitOptions.RemoveEmptyEntries).First();
        return new ExtractedInfo(destinationFolder, rootFolderName, localDlls);
    }

    public static bool FilterWindowsDlls(string x)
    {
        return (x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || x.EndsWith("cache.json")) &&
            !(x.Contains(@"/Debug/", StringComparison.OrdinalIgnoreCase) || x.Contains("_debug.", StringComparison.OrdinalIgnoreCase));
    }
}
