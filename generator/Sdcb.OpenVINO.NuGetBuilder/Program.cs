using Microsoft.Extensions.DependencyInjection;
using NuGet.Versioning;
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
        string purpose = args.Length > 0 ? args[0] : "custom";
        string? versionSuffix = "preview.1"; // preview.1
        string dir = Path.Combine(DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo("."), "OpenVINO.NET.sln").DirectoryName!,
            "build", "nupkgs");

        switch (purpose)
        {
            case "win64":
                await Build_Win_x64(w, root, versionSuffix, dir);
                break;
            case "linux":
                await Build_Linuxs(w, root, versionSuffix, dir);
                break;
            case "custom":
                Build_Custom(versionSuffix, dir);
                break;
            default:
                throw new ArgumentException($"Unknown purpose: {purpose}");
        }
    }

    private static void Build_Custom(string? versionSuffix, string dir)
    {
        NuGetPackageInfo pkgInfo = new(NuGetPackageInfo.GetNamePrefix(), "android-arm64", new SemanticVersion(2023, 1, 0));
        string destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "2023.1");
        ExtractedInfo local = new(destinationFolder, "", Directory.EnumerateFiles(destinationFolder, "*.so", SearchOption.TopDirectoryOnly).ToArray());
        PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
    }

    private static async Task Build_Win_x64(ArtifactDownloader w, StorageNodeRoot root, string? versionSuffix, string dir)
    {
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == KnownOS.Windows);
        NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifact);
        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), pkgInfo.Rid);
        ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new WindowsLibFilter(), flatten: true);
        PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
    }

    private static async Task Build_Linuxs(ArtifactDownloader w, StorageNodeRoot root, string? versionSuffix, string dir)
    {
        foreach (ArtifactInfo artifact in root.LatestStableVersion.Artifacts.Where(x => x.OS == KnownOS.Linux))
        {
            NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifact);
            string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), $"{pkgInfo.Rid}");
            ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new LinuxLibFilter(pkgInfo.Version), flatten: true);
            PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
        }
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