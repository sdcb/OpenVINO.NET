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
    HashSet<string> _libs = new[]
    {
        "libgna.so.3",
        "libopenvino.so.2302",
        "libopenvino_c.so.2302",
        "libopenvino_gapi_preproc.so",
        "libopenvino_auto_batch_plugin.so",
        "libopenvino_auto_plugin.so",
        "libopenvino_hetero_plugin.so",
        "libopenvino_intel_cpu_plugin.so",
        "libopenvino_intel_gna_plugin.so",
        "libopenvino_intel_gpu_plugin.so",
        "libopenvino_ir_frontend.so",
        "libopenvino_onnx_frontend.so",
        "libopenvino_paddle_frontend.so",
        "libopenvino_pytorch_frontend.so",
        "libopenvino_tensorflow_frontend.so",
        "libopenvino_tensorflow_lite_frontend.so"
    }.ToHashSet();

    public bool Filter(string key)
    {
        string fileName = Path.GetFileName(key);
        return _libs.Contains(fileName);
    }
}