using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.NuGetBuilder;
using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;
using SharpCompress.Archives;
using SharpCompress.Factories;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Sdcb.OpenVINO.NuGetBuilder.Tests")]
class Program
{
    static async Task Main()
    {
        IServiceProvider sp = ConfigureServices();
        WindowsSourceExtractor w = sp.GetRequiredService<WindowsSourceExtractor>();
        StorageNodeRoot root = sp.GetRequiredService<StorageNodeRoot>();
        ArtifactInfo artifact = root.LatestStableVersion.Artifacts.First(x => x.OS == KnownOS.Windows);
        await w.DownloadDynamicLibs(artifact);
    }

    static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton<ICachedHttpGetService>(_ => new CachedHttpGetService("cache"))
            .AddSingleton(sp => StorageNodeRoot.LoadRootFromHttp(sp).GetAwaiter().GetResult())
            .AddSingleton<WindowsSourceExtractor>()
            .BuildServiceProvider();
    }
}