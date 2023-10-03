using Sdcb.OpenVINO.Natives;
using Sdcb.OpenVINO.Tests.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    //[Fact]
    //public void CanCompileWithProps()
    //{
    //    using OVCore c = new();
    //    using CompiledModel m = c.CompileModel(_modelFile, properties: new Dictionary<string, string>
    //    {
    //        ["INFERENCE_NUM_THREADS"] = "1",
    //    });
    //    Assert.NotNull(m);
    //}

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
        OpenVINOException ex = Assert.Throws<OpenVINOException>(() => m.GetProperty("not-exists"));
        Assert.Equal(ov_status_e.GENERAL_ERROR, ex.Status);
    }
}
