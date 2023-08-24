

using Sdcb.OpenVINO.NuGetBuilder.Utils;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public static class TransformWriter
{
    public static string DestinationFolder { get; } = Path.Combine(DirectoryUtils.SearchFileByParents(new DirectoryInfo(Environment.CurrentDirectory), "OpenVINO.NET.sln"),
        "generator", $"{nameof(Sdcb)}.{nameof(OpenVINO)}", "Natives");
}
