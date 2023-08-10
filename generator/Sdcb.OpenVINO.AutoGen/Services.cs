using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sdcb.OpenVINO.AutoGen;

internal class Services
{
    public static IServiceProvider ConfigureServices()
    {
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        return new ServiceCollection()
            .AddSingleton<Workflow>()
            .AddSingleton(config.Get<AppSettings>() ?? throw new Exception("Some required config does not exists in appsettings.json"))
            .BuildServiceProvider();
    }
}
