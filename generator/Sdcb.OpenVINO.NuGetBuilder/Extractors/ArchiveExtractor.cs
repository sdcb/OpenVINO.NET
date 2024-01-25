using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilder.Utils;

namespace Sdcb.OpenVINO.NuGetBuilders.Extractors;

public class ArchiveExtractor
{
    public static ExtractedInfo Extract(Stream stream, string destinationFolder, ILibFilter keyFilter, bool flatten)
    {
        using Stream _ = stream;
        ZipContainer reader = ZipContainer.Open(stream);
        ZipEntrySlim[] dynamicLibs = reader.Entries
            .Where(x => keyFilter.Filter(x.Key))
            .ToArray();
        HashSet<string> knownPrefixes = dynamicLibs.Select(x => PathUtils.GetPart1FileNameWithoutExtension(x.Key)).ToHashSet();

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
            foreach (ZipEntrySlim entry in dynamicLibs)
            {
                string destFile = Path.Combine(destinationFolder, flatten ? Path.GetFileName(entry.Key) : entry.Key.Replace(reader.RootFolderName, ""));
                Directory.CreateDirectory(Path.GetDirectoryName(destFile)!);

                if (entry.Data.Length > 0)
                {
                    Console.WriteLine($"{entry.Key}...");
                    File.WriteAllBytes(destFile, entry.Data);
                }
                else if (entry.LinkTarget is not null)
                {
                    // seems like a link
                    ZipEntrySlim? linkedRoot = entry.GetLinkedRoot() ?? throw new Exception($"Entry failed to find link: {entry.Key}.");
                    File.WriteAllBytes(destFile, linkedRoot.Data);
                }
            }
        }

        return new ExtractedInfo(destinationFolder, reader.RootFolderName, localDlls);
    }
}
