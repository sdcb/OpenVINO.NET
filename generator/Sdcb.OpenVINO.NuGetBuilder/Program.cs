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
        string purpose = args.Length > 0 ? args[0] : "win64";
        string? versionSuffix = "preview.1"; // null or "preview.1", can't be ""
        string dir = Path.Combine(DirectoryUtils.SearchFileInCurrentAndParentDirectories(new DirectoryInfo("."), "OpenVINO.NET.sln").DirectoryName!,
            "build", "nupkgs");
        VersionFolder vf = root.LatestStableVersion;

        switch (purpose)
        {
            case "win64":
                await Build_Win_x64(w, vf, versionSuffix, dir);
                break;
            case "linux":
                await Build_Linuxs(w, vf, versionSuffix, dir);
                break;
            case "android":
                BuildAndroid(versionSuffix, dir);
                break;
            case "custom":
                await BuildCustom(w, versionSuffix, dir);
                break;
            default:
                throw new ArgumentException($"Unknown purpose: {purpose}");
        }
    }

    private static async Task BuildCustom(ArtifactDownloader w, string? versionSuffix, string dir)
    {
        ArtifactInfo artifact = new (
            KnownOS.Windows, 
            "windows",
            "x86_64", 
            SemanticVersion.Parse("2023.2.20231110"), 
            new DateTime(2023, 11, 10), 
            "zip",
            "https://io.starworks.cc:88/paddlesharp/ov-lib/w_openvino_toolkit_windows_2023.2.0.dev20231110_x86_64.zip", 
            null);
        NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifact);
        string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), pkgInfo.TitleRid);
        ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new WindowsLibFilter(), flatten: true);
        PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
    }

    private static void BuildAndroid(string? versionSuffix, string dir)
    {
        NuGetPackageInfo pkgInfo = new(NuGetPackageInfo.GetNamePrefix(), "android-arm64", "android-arm64", new SemanticVersion(2023, 1, 0));
        string destinationFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", "android-arm64", "2023.1");
        ExtractedInfo local = new(destinationFolder, "", Directory.EnumerateFiles(destinationFolder, "*.so", SearchOption.TopDirectoryOnly).ToArray());
        PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
    }

    private static async Task Build_Win_x64(ArtifactDownloader w, VersionFolder root, string? versionSuffix, string dir)
    {
        foreach (ArtifactInfo artifact in root.Artifacts.Where(x => x.OS == KnownOS.Windows && !x.DownloadUrl.Contains("dev")))
        {
            NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifact);
            string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), pkgInfo.TitleRid);
            ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new WindowsLibFilter(), flatten: true);
            PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
        }
    }

    private static async Task Build_Linuxs(ArtifactDownloader w, VersionFolder root, string? versionSuffix, string dir)
    {
        foreach (ArtifactInfo artifact in root.Artifacts.Where(x => x.OS == KnownOS.Linux))
        {
            NuGetPackageInfo pkgInfo = NuGetPackageInfo.FromArtifact(artifact);
            string destinationFolder = Path.Combine(new DirectoryInfo(Environment.CurrentDirectory).ToString(), $"{pkgInfo.TitleRid}");
            ExtractedInfo local = await w.DownloadAndExtract(artifact, destinationFolder, new LinuxLibFilter(pkgInfo.Version), flatten: true);
            PackageBuilder.BuildNuGet(local, pkgInfo, versionSuffix, dir);
        }
    }

    static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<ICachedHttpGetService>(_ => new CachedHttpGetService("cache"))
            .AddSingleton(sp => StorageNodeRoot.LoadRootFromHttp(sp).GetAwaiter().GetResult())
            .AddSingleton<ArtifactDownloader>()
            .BuildServiceProvider();
    }
}