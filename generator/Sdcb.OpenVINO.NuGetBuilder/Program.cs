using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilders;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;
using Sdcb.OpenVINO.NuGetBuilders.PackageBuilder;
using Sdcb.OpenVINO.NuGetBuilders.Utils;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.OpenVINO.NuGetBuilder.Tests")]
class Program
{
    static async Task Main(string[] args)
    {
        IServiceProvider sp = ConfigureServices();
        ArtifactDownloader w = sp.GetRequiredService<ArtifactDownloader>();
        StorageNodeRoot root = sp.GetRequiredService<StorageNodeRoot>();
        string purpose = args.Length > 0 ? args[0] : "ubuntu22-x64";
        string? versionSuffix = "preview.1";
        string dir = Path.Combine(DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo("."), "OpenVINO.NET.sln").DirectoryName!,
            "build", "nupkgs");

        switch (purpose)
        {
            case "win64":
                await Build_Win_x64(w, root, versionSuffix, dir);
                break;
            case "ubuntu22-x64":
                await Build_Ubuntu22_x64(w, root, versionSuffix, dir);
                break;
            default:
                throw new ArgumentException($"Unknown purpose: {purpose}");
        }
    }

    private static async Task Build_Win_x64(ArtifactDownloader w, StorageNodeRoot root, string versionSuffix, string dir)
    {
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == KnownOS.Windows);
        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), artifact.Distribution);
        ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new WindowsLibFilter(), flatten: true);
        PackageBuilder.BuildNuGet(local, artifact, versionSuffix, dir);
    }

    private static async Task Build_Ubuntu22_x64(ArtifactDownloader w, StorageNodeRoot root, string versionSuffix, string dir)
    {
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.Distribution == "ubuntu22" && x.Arch == "x86_64");
        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), artifact.Distribution);
        ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new LinuxLibFilter(), flatten: true);
        PackageBuilder.BuildNuGet(local, artifact, versionSuffix, dir);
    }

    static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<ICachedHttpGetService>(_ => new CachedHttpGetService("cache"))
            .AddSingleton(sp => StorageNodeRoot.LoadRootFromHttp(sp).GetAwaiter().GetResult())
            .AddSingleton<ArtifactDownloader>()
            .BuildServiceProvider();
    }
}