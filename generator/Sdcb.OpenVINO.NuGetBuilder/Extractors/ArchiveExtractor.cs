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
    public static ExtractedInfo Extract(Stream stream, string destinationFolder, Func<string, bool> keyFilter, bool flatten)
    {
        using Stream _ = stream;
        using ArchiveReader reader = ArchiveReader.Open(stream);
        IArchiveEntry[] dynamicLibs = reader.Entries
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

        return new ExtractedInfo(destinationFolder, reader.RootFolderName, localDlls);
    }

    public static bool FilterWindowsDlls(string x)
    {
        return (x.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || x.EndsWith("cache.json")) &&
            !(x.Contains(@"/Debug/", StringComparison.OrdinalIgnoreCase) || x.Contains("_debug.", StringComparison.OrdinalIgnoreCase));
    }

    public static bool FilterLinuxDlls(string x)
    {
        HashSet<string> libs = new[]
        {
            "libgna.so.3",
            "libopenvino.so.2302",
            "libopenvino_c.so.2302",
            "libopenvino_gapi_preproc.so",
            "libopenvino_auto_batch_plugin.so",
            "libopenvino_auto_plugin.so",
            "libopenvino_hetero_plugin.so",
            "libopenvino_intel_cpu_plugin.so",
            "libopenvino_intel_gna_plugin.so",
            "libopenvino_intel_gpu_plugin.so",
            "libopenvino_ir_frontend.so",
            "libopenvino_onnx_frontend.so",
            "libopenvino_paddle_frontend.so",
            "libopenvino_pytorch_frontend.so",
            "libopenvino_tensorflow_frontend.so",
            "libopenvino_tensorflow_lite_frontend.so"
        }.ToHashSet();

        return libs.Any(lib => lib.EndsWith(x));
    }
}
