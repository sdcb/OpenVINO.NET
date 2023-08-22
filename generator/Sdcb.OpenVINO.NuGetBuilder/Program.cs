using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.NuGetBuilder;
using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using Sdcb.OpenVINO.NuGetBuilder.PackageBuilder;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.OpenVINO.NuGetBuilder.Tests")]
class Program
{
    static async Task Main()
    {
        IServiceProvider sp = ConfigureServices();
        WindowsSourceExtractor w = sp.GetRequiredService<WindowsSourceExtractor>();
        WindowsPackageBuilder b = sp.GetRequiredService<WindowsPackageBuilder>();
        StorageNodeRoot root = sp.GetRequiredService<StorageNodeRoot>();
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == KnownOS.Windows);
        ExtractedInfo local = await w.DownloadDynamicLibs(artifact);
        b.BuildNuGet(local, artifact);
    }

    static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<ICachedHttpGetService>(_ => new CachedHttpGetService("cache"))
            .AddSingleton(sp => StorageNodeRoot.LoadRootFromHttp(sp).GetAwaiter().GetResult())
            .AddSingleton<WindowsSourceExtractor>()
            .AddSingleton<WindowsPackageBuilder>()
            .BuildServiceProvider();
    }
}