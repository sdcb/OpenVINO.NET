using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.NuGetBuilder;
using Sdcb.OpenVINO.NuGetBuilder.ArtifactSources;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        IServiceProvider sp = ConfigureServices();
        Debug(sp);
        
    }

    private static void Debug(IServiceProvider sp)
    {
        OpenVINOFileTreeRoot repo = sp.GetRequiredService<OpenVINOFileTreeRoot>();
    }

    static IServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(_ => new CachedHttpGetService("cache"))
            .AddSingleton(sp => OpenVINOFileTreeRoot.LoadRootAsync(sp).GetAwaiter().GetResult())
            .AddSingleton<ArtifactRepository>()
            .BuildServiceProvider();
    }
}