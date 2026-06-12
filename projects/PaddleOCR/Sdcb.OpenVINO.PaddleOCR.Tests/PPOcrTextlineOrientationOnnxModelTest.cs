using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class PPOcrTextlineOrientationOnnxModelTest
{
    private readonly ITestOutputHelper _console;

    public PPOcrTextlineOrientationOnnxModelTest(ITestOutputHelper console)
    {
        _console = console;
    }

    public static TheoryData<string> Models => new()
    {
        nameof(OnlineOnnxClassificationModel.TextLineOrientationX025),
        nameof(OnlineOnnxClassificationModel.TextLineOrientationX10),
    };

    [Theory]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    [MemberData(nameof(Models))]
    public async Task LoadAndCompileTextlineOrientationOnnxModel(string modelName)
    {
        ClassificationModel classificationModel = await GetModel(modelName).DownloadAsync();

        using OVCore core = new();
        _console.WriteLine($"OpenVINO: {OVCore.Version}");

        Assert.Equal(ClassificationResizeMode.DirectResize, classificationModel.ResizeMode);

        using Model model = classificationModel.CreateOVModel(core);
        DumpModelPorts(modelName, model);

        Assert.NotEmpty(model.Inputs);
        Assert.NotEmpty(model.Outputs);

        using CompiledModel compiledModel = classificationModel.CreateCompiledModel(new DeviceOptions("CPU"));

        Assert.Equal(model.Inputs.Count, compiledModel.Inputs.Count);
        Assert.Equal(model.Outputs.Count, compiledModel.Outputs.Count);
    }

    [Theory]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    [MemberData(nameof(Models))]
    public async Task RunTextlineOrientationOnnxModel(string modelName)
    {
        using Mat upright = Cv2.ImRead("./samples/5ghz.jpg");
        using Mat rotated = new();
        Cv2.Rotate(upright, rotated, RotateFlags.Rotate180);

        using PaddleOcrClassifier classifier = new(await GetModel(modelName).DownloadAsync(), new DeviceOptions("CPU"));

        Ocr180DegreeClsResult[] results = classifier.ShouldRotate180(new[] { upright, rotated });
        for (int i = 0; i < results.Length; ++i)
        {
            _console.WriteLine($"{modelName}[{i}]: shouldRotate={results[i].ShouldRotate180}, confidence={results[i].Confidence}");
        }

        Assert.False(results[0].ShouldRotate180);
        Assert.True(results[1].ShouldRotate180);
    }

    private static OnlineOnnxClassificationModel GetModel(string modelName)
    {
        return modelName switch
        {
            nameof(OnlineOnnxClassificationModel.TextLineOrientationX025) => OnlineOnnxClassificationModel.TextLineOrientationX025,
            nameof(OnlineOnnxClassificationModel.TextLineOrientationX10) => OnlineOnnxClassificationModel.TextLineOrientationX10,
            _ => throw new ArgumentOutOfRangeException(nameof(modelName), modelName, null),
        };
    }

    private void DumpModelPorts(string modelName, Model model)
    {
        _console.WriteLine($"{modelName}: inputs={model.Inputs.Count}, outputs={model.Outputs.Count}");
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
