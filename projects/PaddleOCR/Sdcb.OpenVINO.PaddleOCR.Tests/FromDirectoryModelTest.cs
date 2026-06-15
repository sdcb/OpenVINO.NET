using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Details;

namespace Sdcb.OpenVINO.PaddleOCR.Tests;

public class FromDirectoryModelTest
{
    [Fact]
    public void DetectionV6FromDirectoryUsesOnnxModel()
    {
        DetectionModel model = DetectionModel.FromDirectory("models/det", ModelVersion.V6);

        Assert.IsType<FileOnnxDetectionModel>(model);
    }

    [Fact]
    public void DetectionNonV6FromDirectoryUsesPaddleModel()
    {
        DetectionModel model = DetectionModel.FromDirectory("models/det", ModelVersion.V4);

        Assert.IsType<FileDetectionModel>(model);
    }

    [Fact]
    public void ClassificationV6FromDirectoryUsesOnnxDefaults()
    {
        ClassificationModel model = ClassificationModel.FromDirectory("models/cls", ModelVersion.V6);

        FileOnnxClassificationModel onnxModel = Assert.IsType<FileOnnxClassificationModel>(model);
        Assert.Equal(new NCHW(-1, 3, 80, 160), onnxModel.Shape);
        Assert.Equal(ClassificationPreprocessMode.ImageNetRgb, onnxModel.PreprocessMode);
        Assert.Equal(ClassificationResizeMode.DirectResize, onnxModel.ResizeMode);
    }

    [Fact]
    public void ClassificationNonV6FromDirectoryUsesPaddleModel()
    {
        ClassificationModel model = ClassificationModel.FromDirectory("models/cls", ModelVersion.V2);

        Assert.IsType<FileClassificationModel>(model);
    }

    [Fact]
    public void RecognizationFromV6DirectoryUsesOnnxInferenceYml()
    {
        string directoryPath = CreateV6RecognitionDirectory();

        RecognizationModel model = RecognizationModel.FromV6Directory(directoryPath);

        FileOnnxRecognizationModel onnxModel = Assert.IsType<FileOnnxRecognizationModel>(model);
        Assert.Equal("A", onnxModel.GetLabelByIndex(1));
    }

    [Fact]
    public void RecognizationNonV6FromDirectoryUsesPaddleModel()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directoryPath);
        string labelPath = Path.Combine(directoryPath, "labels.txt");
        File.WriteAllText(labelPath, "A");

        RecognizationModel model = RecognizationModel.FromDirectory(directoryPath, labelPath, ModelVersion.V4);

        Assert.IsType<FileRecognizationModel>(model);
    }

    [Fact]
    public void RecognizationFromDirectoryKeepsPaddleModelSemanticsForV6()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directoryPath);
        string labelPath = Path.Combine(directoryPath, "labels.txt");
        File.WriteAllText(labelPath, "A");

        RecognizationModel model = RecognizationModel.FromDirectory(directoryPath, labelPath, ModelVersion.V6);

        Assert.IsType<FileRecognizationModel>(model);
    }

    private static string CreateV6RecognitionDirectory()
    {
        string directoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(directoryPath);
        File.WriteAllText(Path.Combine(directoryPath, "inference.yml"),
            "PostProcess:\n" +
            "  character_dict:\n" +
            "  - A\n");
        return directoryPath;
    }
}
