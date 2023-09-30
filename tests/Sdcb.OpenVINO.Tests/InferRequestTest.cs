using OpenCvSharp;
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
    public void CanCreate()
    {
        using OVCore c = new();
        using CompiledModel cm = c.CompileModel(_modelFile);
        using InferRequest r = cm.CreateInferRequest();
        float[] data = new float[960 * 960];
        r.Inputs.Primary = Tensor.FromArray(data, new Shape(960, 960, 3));
    }
}
