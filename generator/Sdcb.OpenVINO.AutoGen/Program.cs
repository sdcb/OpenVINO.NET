using Microsoft.Extensions.DependencyInjection;
using Sdcb.OpenVINO.AutoGen;


IServiceProvider services = Services.ConfigureServices();
await services.GetRequiredService<Workflow>().Run();