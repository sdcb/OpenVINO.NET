using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.AutoGen;
using Sdcb.OpenVINO.AutoGen.Downloader;
using Sdcb.OpenVINO.AutoGen.Parsers;
using Sdcb.OpenVINO.NuGetBuilders.ArtifactSources;
using Sdcb.OpenVINO.NuGetBuilders;
using Sdcb.OpenVINO.NuGetBuilders.Extractors;
using Sdcb.OpenVINO.AutoGen.Headers;
using Sdcb.OpenVINO.AutoGen.Writers;

IServiceProvider services = ConfigureServices();
ExtractedInfo info = (await services.GetRequiredService<HeadersDownloader>().DownloadAsync());
ParsedInfo parsed = HeadersParser.Parse(info);
GeneratedAll all = GeneratedAll.Generate(parsed);
TransformWriter.WriteAll(all, TransformWriter.DestinationFolder, "Sdcb.OpenVINO.Natives");

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
        .BuildServiceProvider();
}