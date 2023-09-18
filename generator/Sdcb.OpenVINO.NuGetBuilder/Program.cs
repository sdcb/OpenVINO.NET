using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.NuGetBuilder;
using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilder.PackageBuilder;
using Sdcb.OpenVINO.NuGetBuilder.Utils;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.OpenVINO.NuGetBuilder.Tests")]
class Program
{
    static async Task Main()
    {
        IServiceProvider sp = ConfigureServices();
        ArtifactDownloader w = sp.GetRequiredService<ArtifactDownloader>();
        StorageNodeRoot root = sp.GetRequiredService<StorageNodeRoot>();
        string? versionSuffix = "preview.1";
        string dir = Path.Combine(DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo("."), "OpenVINO.NET.sln").DirectoryName!,
            "build", "nupkgs");
        await Build_WinX64(w, root, versionSuffix, dir);
    }

    private static async Task Build_WinX64(ArtifactDownloader w, StorageNodeRoot root, string versionSuffix, string dir)
    {
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == KnownOS.Windows);
        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), artifact.Distribution);
        ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, ArchiveExtractor.FilterWindowsDlls, flatten: true);
        WindowsPackageBuilder.BuildNuGet(local, artifact, versionSuffix, dir);
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