using NuGet.Versioning;

namespace Sdcb.OpenVINO.NuGetBuilder.Extractors;

public interface ILibFilter
{
    bool Filter(string key);
}

public class WindowsLibFilter : ILibFilter
{
    public bool Filter(string key)
    {
        return (key.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || key.EndsWith("cache.json")) &&
            !(key.Contains(@"/Debug/", StringComparison.OrdinalIgnoreCase) || key.Contains("_debug.", StringComparison.OrdinalIgnoreCase));
    }
}

public class LinuxLibFilter : ILibFilter
{
    public LinuxLibFilter(SemanticVersion version)
    {
        string suffix = version.Major.ToString()[^2..] + version.Minor + version.Patch;
        _libs.Add($"libopenvino.so.{suffix}");
        _libs.Add($"libopenvino_c.so.{suffix}");
        _libs.Add($"libopenvino_ir_frontend.so.{suffix}");
    }

    HashSet<string> _libs = new[]
    {
        "libgna.so.3",
        "libnpu_driver_compiler_adapter.so",
        "libnpu_level_zero_backend.so",
        //"libopenvino.so.2302", // added in constructor
        //"libopenvino_c.so.2302", // added in constructor
        "libopenvino_gapi_preproc.so",
        "libopenvino_auto_batch_plugin.so",
        "libopenvino_auto_plugin.so",
        "libopenvino_hetero_plugin.so",
        "libopenvino_arm_cpu_plugin.so",
        "libopenvino_intel_cpu_plugin.so",
        "libopenvino_intel_gna_plugin.so",
        "libopenvino_intel_gpu_plugin.so",
        "libopenvino_intel_npu_plugin.so",
        //"libopenvino_ir_frontend.so.2302", // added in constructor
        "libopenvino_onnx_frontend.so",
        "libopenvino_paddle_frontend.so",
        "libopenvino_pytorch_frontend.so",
        "libopenvino_tensorflow_frontend.so",
        "libopenvino_tensorflow_lite_frontend.so",
        "cache.json",
        "plugins.xml",
    }.ToHashSet();

    public bool Filter(string key)
    {
        string fileName = Path.GetFileName(key);
        return _libs.Contains(fileName);
    }
}