using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class PPOcrDocumentOrientationOnnxModelTest
{
    private readonly ITestOutputHelper _console;

    public PPOcrDocumentOrientationOnnxModelTest(ITestOutputHelper console)
    {
        _console = console;
    }

    [Fact]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    public async Task LoadAndCompileDocumentOrientationOnnxModel()
    {
        DocumentOrientationClassificationModel orientationModel = await OnlineDocumentOrientationClassificationModel.PPDocOrientationX10.DownloadAsync();

        using OVCore core = new();
        _console.WriteLine($"OpenVINO: {OVCore.Version}");

        using Model model = orientationModel.CreateOVModel(core);
        DumpModelPorts(model);

        Assert.NotEmpty(model.Inputs);
        Assert.NotEmpty(model.Outputs);

        using CompiledModel compiledModel = orientationModel.CreateCompiledModel(new DeviceOptions("CPU"));

        Assert.Equal(model.Inputs.Count, compiledModel.Inputs.Count);
        Assert.Equal(model.Outputs.Count, compiledModel.Outputs.Count);
    }

    [Fact]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    public async Task RunDocumentOrientationOnnxModel()
    {
        using PaddleOcrDocumentOrientationClassifier classifier = new(
            await OnlineDocumentOrientationClassificationModel.PPDocOrientationX10.DownloadAsync(),
            new DeviceOptions("CPU"));

        using Mat src0 = Cv2.ImRead("./samples/table.jpg");
        using Mat src90 = new();
        using Mat src180 = new();
        using Mat src270 = new();
        Cv2.Rotate(src0, src90, RotateFlags.Rotate90Clockwise);
        Cv2.Rotate(src0, src180, RotateFlags.Rotate180);
        Cv2.Rotate(src0, src270, RotateFlags.Rotate90Counterclockwise);

        AssertRotation(classifier, src0, 0);
        AssertRotation(classifier, src90, 90);
        AssertRotation(classifier, src180, 180);
        AssertRotation(classifier, src270, 270);
    }

    private void AssertRotation(PaddleOcrDocumentOrientationClassifier classifier, Mat src, int expected)
    {
        PaddleOcrDocumentOrientationResult result = classifier.Run(src);
        _console.WriteLine($"expected={expected}, {result}");
        Assert.Equal(expected, result.Angle);
    }

    private void DumpModelPorts(Model model)
    {
        _console.WriteLine($"inputs={model.Inputs.Count}, outputs={model.Outputs.Count}");
        for (int i = 0; i < model.Inputs.Count; ++i)
        {
            using IOPort input = model.Inputs[i];
            _console.WriteLine($"  input[{i}] {input.Name}: {input.PartialShape}, {input.ElementType}");
        }

        for (int i = 0; i < model.Outputs.Count; ++i)
        {
            using IOPort output = model.Outputs[i];
            _console.WriteLine($"  output[{i}] {output.Name}: {output.PartialShape}, {output.ElementType}");
        }
    }
}
