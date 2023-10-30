using Sdcb.OpenVINO.Tests.Natives;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class CompiledModelTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public CompiledModelTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = OVCoreNativeTest.PrepareModel();
    }

    [Fact]
    public void CanCompile()
    {
        using OVCore c = new();
        using CompiledModel m = c.CompileModel(_modelFile);
        Assert.NotNull(m);
    }

    [Fact]
    public void CanCompileWithProps()
    {
        using OVCore c = new();
        using CompiledModel m = c.CompileModel(_modelFile, "CPU", properties: new Dictionary<string, string>
        {
            ["INFERENCE_NUM_THREADS"] = "4",
            ["NUM_STREAMS"] = "2",
        });
        Assert.NotNull(m);
        Assert.Equal("4", m.Properties["INFERENCE_NUM_THREADS"]);
        Assert.Equal("2", m.Properties["NUM_STREAMS"]);
    }

    [Fact]
    public void CanCompileWithDeviceOptions()
    {
        using OVCore c = new();
        using CompiledModel m = c.CompileModel(_modelFile, new DeviceOptions()
        {
            PerformanceMode = PerformanceMode.CumulativeThroughput,
        });
        Assert.NotNull(m);
        Assert.Equal("CUMULATIVE_THROUGHPUT", m.Properties[PropertyKeys.HintPerformanceMode]);
    }

    [Fact]
    public void CanCompileExistingModelWithDictProps()
    {
        using OVCore c = new();
        using Model rawModel = c.ReadModel(_modelFile);
        using CompiledModel m = c.CompileModel(rawModel, "CPU", properties: new Dictionary<string, string>
        {
            ["INFERENCE_NUM_THREADS"] = "4",
            ["NUM_STREAMS"] = "2",
        });
        Assert.NotNull(m);
        Assert.Equal("4", m.Properties["INFERENCE_NUM_THREADS"]);
        Assert.Equal("2", m.Properties["NUM_STREAMS"]);
    }

    [Fact]
    public void CanCompileExistingModelWithProps()
    {
        using OVCore c = new();
        using Model rawModel = c.ReadModel(_modelFile);
        using CompiledModel m = c.CompileModel(rawModel, new DeviceOptions("CPU")
        {
            SchedulingCoreType = SchedulingCoreType.PCoresOnly, 
            EnableHyperThreading = true, 
            InferenceNumThreads = 4, 
            NumStreams = 2, 
            PerformanceMode = PerformanceMode.Throughput
        });
        Assert.NotNull(m);
        Assert.Equal("4", m.Properties[PropertyKeys.InferenceNumThreads]);
        Assert.Equal("2", m.Properties[PropertyKeys.NumStreams]);
        Assert.Equal("YES", m.Properties[PropertyKeys.HintEnableHyperThreading]);
        Assert.Equal("PCORE_ONLY", m.Properties[PropertyKeys.HintSchedulingCoreType]);
        Assert.Equal("THROUGHPUT", m.Properties[PropertyKeys.HintPerformanceMode]);
    }

    [Fact]
    public void CanGetAllSupportedProp()
    {
        using OVCore c = new();
        using CompiledModel m = c.CompileModel(_modelFile);
        foreach (var prop in m.Properties)
        {
            _console.WriteLine($"{prop.Key}: {prop.Value}");
        }
    }

    [Fact]
    public void CanNotGetProp()
    {
        using OVCore c = new();
        using CompiledModel m = c.CompileModel(_modelFile);
        KeyNotFoundException ex = Assert.Throws<KeyNotFoundException>(() => m.Properties["not-exists"]);
    }
}
