using Sdcb.OpenVINO.Natives;
using Sdcb.OpenVINO.Tests.Natives;
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

    [Fact(Skip = "Async is not working")]
    public async Task MinInferAsync()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = Tensor.FromArray(new float[32 * 64 * 3], new Shape(1, 3, 32, 64));
        r.Inputs.Primary = input;

        await r.RunAsync();

        //using Tensor output = r.Outputs.Primary;
        //Assert.Equal(new Shape(1, 1, 32, 64), output.Shape);
        //Assert.Equal(32 * 64, output.GetData<float>().Length);
        //Assert.Equal(ov_element_type_e.F32, output.ElementType);
    }

    [Fact]
    public void TranditionalAsync()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = Tensor.FromArray(new float[32 * 64 * 3], new Shape(1, 3, 32, 64));
        r.Inputs.Primary = input;

        r.StartAsyncRun();
        r.WaitAsyncRun();

        using Tensor output = r.Outputs.Primary;
        Assert.Equal(new Shape(1, 1, 32, 64), output.Shape);
        Assert.Equal(32 * 64, output.GetData<float>().Length);
        Assert.Equal(ov_element_type_e.F32, output.ElementType);
    }

    [Fact]
    public void WaitSuccess()
    {
        if (!OpenVINOLibraryLoader.Is202302OrGreater())
        {
            _console.WriteLine($"Version < 2023.2 not support determin wait success or not");
            return;
        }

        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        using Tensor input = Tensor.FromArray(new float[320 * 256 * 3], new Shape(1, 3, 320, 256));
        r.Inputs.Primary = input;

        r.StartAsyncRun();
        bool wait1ms = r.WaitAsyncRun(TimeSpan.FromMilliseconds(1));
        bool wait1m = r.WaitAsyncRun(TimeSpan.FromMinutes(1));

        Assert.False(wait1ms);
        Assert.True(wait1m);
    }

    [Fact]
    public void IRShouldLimitTensorCount()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        Assert.Throws<IndexOutOfRangeException>(() => r.Inputs[4]);
    }

    [Fact(Skip = "Cancel after 1ms is not reliable")]
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
