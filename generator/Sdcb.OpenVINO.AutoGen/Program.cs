using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.AutoGen;
using Sdcb.OpenVINO.AutoGen.Downloader;
using Sdcb.OpenVINO.AutoGen.Parsers;
using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilder;
using Sdcb.OpenVINO.NuGetBuilder.Extractors;

IServiceProvider services = ConfigureServices();
ExtractedInfo info = (await services.GetRequiredService<HeadersDownloader>().DownloadAsync());
services.GetRequiredService<HeadersParser>().Run(info);


static IServiceProvider ConfigureServices()
{
    IConfiguration config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    return new ServiceCollection()
        .AddSingleton(config.Get<AppSettings>() ?? throw new Exception("Some required config does not exists in appsettings.json"))
        .AddSingleton<ICachedHttpGetService>(sp => new CachedHttpGetService(sp.GetRequiredService<AppSettings>().DownloadFolder))
        .AddSingleton(sp => StorageNodeRoot.LoadRootFromHttp(sp).GetAwaiter().GetResult())
        .AddSingleton<ArtifactDownloader>()
        .AddSingleton<HeadersDownloader>()
        .AddSingleton<HeadersParser>()
        .BuildServiceProvider();
}