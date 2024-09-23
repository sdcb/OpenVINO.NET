using System.Runtime.Versioning;
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

[assembly: SupportedOSPlatform("windows")]

IServiceProvider services = ConfigureServices();
//ExtractedInfo info = (await services.GetRequiredService<HeadersDownloader>().DownloadAsync());
AppSettings appSettings = services.GetRequiredService<AppSettings>();
string url = "https://storage.openvinotoolkit.org/repositories/openvino/packages/2024.3/windows/w_openvino_toolkit_windows_2024.3.0.16041.1e3b88e4e3f_x86_64.zip";
ExtractedInfo info = await HeadersDownloader.DirectDownloadAsync(url, services.GetRequiredService<ArtifactDownloader>(), appSettings.DownloadFolder);
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