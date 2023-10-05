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
        using CompiledModel m = c.CompileModel(_modelFile, properties: new Dictionary<string, string>
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
        using CompiledModel m = c.CompileModel(rawModel, properties: new Dictionary<string, string>
        {
            ["INFERENCE_NUM_THREADS"] = "4",
            ["NUM_STREAMS"] = "2",
        });
        Assert.NotNull(m);
        Assert.Equal("4", m.Properties["INFERENCE_NUM_THREADS"]);
        Assert.Equal("2", m.Properties["NUM_STREAMS"]);
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
