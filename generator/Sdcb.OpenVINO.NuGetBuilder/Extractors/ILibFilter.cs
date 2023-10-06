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

        _libs = new[]
        {
            "libgna.so.3",
            "libnpu_driver_compiler_adapter.so",
            "libnpu_level_zero_backend.so",
            $"libopenvino.so.{suffix}", 
            $"libopenvino_c.so.{suffix}", 
            "libopenvino_gapi_preproc.so",
            "libopenvino_auto_batch_plugin.so",
            "libopenvino_auto_plugin.so",
            "libopenvino_hetero_plugin.so",
            "libopenvino_arm_cpu_plugin.so",
            "libopenvino_intel_cpu_plugin.so",
            "libopenvino_intel_gna_plugin.so",
            "libopenvino_intel_gpu_plugin.so",
            "libopenvino_intel_npu_plugin.so",
            $"libopenvino_ir_frontend.so.{suffix}", 
            $"libopenvino_onnx_frontend.so.{suffix}",
            $"libopenvino_paddle_frontend.so.{suffix}",
            $"libopenvino_pytorch_frontend.so.{suffix}",
            $"libopenvino_tensorflow_frontend.so.{suffix}",
            $"libopenvino_tensorflow_lite_frontend.so.{suffix}",
            "cache.json",
            "plugins.xml",
        }.ToHashSet();
    }

    private readonly HashSet<string> _libs;

    public bool Filter(string key)
    {
        string fileName = Path.GetFileName(key);
        return _libs.Contains(fileName);
    }
}