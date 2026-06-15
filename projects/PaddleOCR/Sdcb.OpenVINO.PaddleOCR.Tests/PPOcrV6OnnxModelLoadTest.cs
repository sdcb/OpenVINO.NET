using OpenCvSharp;
using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Online;
using Xunit.Abstractions;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class PPOcrV6OnnxModelLoadTest
{
    private readonly ITestOutputHelper _console;

    public PPOcrV6OnnxModelLoadTest(ITestOutputHelper console)
    {
        _console = console;
    }

    public static TheoryData<string> FullModels => new()
    {
        nameof(OnlineFullModels.ChineseV6Small),
        nameof(OnlineFullModels.ChineseV6Tiny),
        nameof(OnlineFullModels.ChineseV6Medium),
    };

    [Theory]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    [MemberData(nameof(FullModels))]
    public async Task DownloadPPOcrV6OnnxFullModel(string modelName)
    {
        FullOcrModel model = await GetFullModel(modelName).DownloadAsync();

        Assert.NotNull(model.DetectionModel);
        Assert.NotNull(model.ClassificationModel);
        Assert.NotNull(model.RecognizationModel);
        Assert.NotNull(model.DocumentOrientationModel);
    }

    [Fact]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    public async Task LoadAndCompilePPOcrV6SmallOnnxModels()
    {
        FullOcrModel fullModel = await OnlineFullModels.ChineseV6Small.DownloadAsync();

        using OVCore core = new();
        _console.WriteLine($"OpenVINO: {OVCore.Version}");

        using Model detModel = fullModel.DetectionModel.CreateOVModel(core);
        DumpModelPorts("det", detModel);
        using CompiledModel detCompiled = fullModel.DetectionModel.CreateCompiledModel(new DeviceOptions("CPU"));

        using Model recModel = fullModel.RecognizationModel.CreateOVModel(core);
        DumpModelPorts("rec", recModel);
        using CompiledModel recCompiled = fullModel.RecognizationModel.CreateCompiledModel(new DeviceOptions("CPU"));

        Assert.NotEmpty(detCompiled.Inputs);
        Assert.NotEmpty(recCompiled.Inputs);
    }

    [Fact]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    public async Task PPOcrV6DetectorUsesOfficialPostProcessDefaults()
    {
        using PaddleOcrDetector detector = new(
            await OnlineDetectionModel.ChineseV6Small.DownloadAsync(),
            new DeviceOptions("CPU"));

        Assert.Equal(0.2f, detector.BoxThreshold.GetValueOrDefault());
        Assert.Equal(0.45f, detector.BoxScoreThreahold.GetValueOrDefault());
        Assert.Equal(1.4f, detector.UnclipRatio);
    }

    [Fact]
    [Trait("Category", "Online")]
    [Trait("Category", "OpenVINO2026")]
    [Trait("Category", "Onnx")]
    public async Task RunPPOcrV6SmallOnnxRealOcr()
    {
        using PaddleOcrAll all = new(await OnlineFullModels.ChineseV6Small.DownloadAsync(), new PaddleOcrOptions(new DeviceOptions("CPU")))
        {
            AllowRotateDetection = true,
            Enable180Classification = false,
            EnableDocumentOrientationClassification = false,
        };
        using Mat src = Cv2.ImRead("./samples/5ghz.jpg");

        PaddleOcrResult result = all.Run(src);
        _console.WriteLine(result.Text);
        foreach (PaddleOcrResultRegion region in result.Regions)
        {
            _console.WriteLine($"Text: {region.Text}, Score: {region.Score}, RectCenter: {region.Rect.Center}, RectSize: {region.Rect.Size}, Angle: {region.Rect.Angle}");
        }

        Assert.Contains("5GHz", result.Text);
        Assert.Contains("频段", result.Text);
    }

    private static OnlineFullModels GetFullModel(string modelName)
    {
        return modelName switch
        {
            nameof(OnlineFullModels.ChineseV6Small) => OnlineFullModels.ChineseV6Small,
            nameof(OnlineFullModels.ChineseV6Tiny) => OnlineFullModels.ChineseV6Tiny,
            nameof(OnlineFullModels.ChineseV6Medium) => OnlineFullModels.ChineseV6Medium,
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
