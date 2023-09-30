using OpenCvSharp;
using Sdcb.OpenVINO.Natives;
using Sdcb.OpenVINO.Tests.Natives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.Tests;

public class InferRequestTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public InferRequestTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = OVCoreNativeTest.PrepareModel();
    }

    [Fact]
    public void MinInfer()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = r.Inputs.Primary;
        input.Shape = new Shape(1, 3, 32, 64);

        r.Run();

        using Tensor output = r.Outputs.Primary;
        Assert.Equal(new Shape(1, 1, 32, 64), output.Shape);
        Assert.Equal(32 * 64, output.GetData<float>().Length);
        Assert.Equal(ov_element_type_e.F32, output.ElementType);
    }

    [Fact]
    public async Task MinInferAsync()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = r.Inputs.Primary;
        input.Shape = new Shape(1, 3, 32, 64);

        await r.RunAsync();

        using Tensor output = r.Outputs.Primary;
        Assert.Equal(new Shape(1, 1, 32, 64), output.Shape);
        Assert.Equal(32 * 64, output.GetData<float>().Length);
        Assert.Equal(ov_element_type_e.F32, output.ElementType);
    }

    [Fact]
    public void MinInferAsyncWithCancel()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = r.Inputs.Primary;
        input.Shape = new Shape(1, 3, 320, 640);

        CancellationTokenSource cts = new(1);
        Task t = r.RunAsync(cts.Token);

        try { t.GetAwaiter().GetResult(); } catch (TaskCanceledException) { }
        Assert.True(t.IsCanceled);
    }
}
