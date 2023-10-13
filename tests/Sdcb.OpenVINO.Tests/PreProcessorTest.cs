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

public class PreProcessorTest
{
    private readonly ITestOutputHelper _console;
    private readonly string _modelFile;

    public PreProcessorTest(ITestOutputHelper console)
    {
        _console = console;
        _modelFile = OVCoreNativeTest.PrepareModel();
    }

    [Fact]
    public void UpdateElementType()
    {
        using OVCore c = new();
        using Model m1 = c.ReadModel(_modelFile);
        using PrePostProcessor pp = m1.CreatePrePostProcessor();
        using PreProcessInputInfo info = pp.Inputs.Primary;
        info.TensorInfo.ElementType = ov_element_type_e.F16;
        using Model m = pp.BuildModel();
        Assert.Equal(ov_element_type_e.F16, m.Inputs.Primary.ElementType);
    }

    [Fact]
    public unsafe void PaddleOCRDetector()
    {
        using Mat src = Cv2.ImRead(@"./assets/text.png");
        using Mat pad32 = MatPadding32(src);
        using Mat f32 = new();
        pad32.ConvertTo(f32, MatType.CV_32FC3, 1.0 / 255);
        using Tensor input = Tensor.FromRaw(
            new ReadOnlySpan<byte>((void*)f32.Data, (int)(f32.DataEnd - f32.DataStart)),
            new Shape(1, f32.Height, f32.Width, 3), 
            ov_element_type_e.F32);

        using OVCore c = OVCore.Shared;
        using Model m1 = c.ReadModel(_modelFile);
        using PrePostProcessor pp = m1.CreatePrePostProcessor();
        using (PreProcessInputInfo info = pp.Inputs.Primary)
        {
            info.TensorInfo.Layout = Layout.NHWC;
            info.ModelInfo.Layout = Layout.NCHW;
        }
        using (PreProcessOutputInfo outputInfo = pp.Outputs.Primary)
        {
        }
        using Model m = pp.BuildModel();
        using CompiledModel cm = c.CompileModel(m);
        using InferRequest r = cm.CreateInferRequest();
        r.Inputs.Primary = input;
        r.Run();
        using Tensor output = r.Outputs.Primary;
        NCHW outputNCHW = output.Shape.ToNCHW();
        using Mat mat = new(outputNCHW.Height, outputNCHW.Width, MatType.CV_32FC1, output.DangerousGetDataPtr());
        using Mat u8 = new();
        mat.ConvertTo(u8, MatType.CV_8SC1, 255);
        //u8.ImWrite("test.png");

        static Mat MatPadding32(Mat src)
        {
            Size size = src.Size();
            Size newSize = new(
                32 * Math.Ceiling(1.0 * size.Width / 32),
                32 * Math.Ceiling(1.0 * size.Height / 32));
            return src.CopyMakeBorder(0, newSize.Height - size.Height, 0, newSize.Width - size.Width, BorderTypes.Constant, Scalar.Black);
        }
    }
}
