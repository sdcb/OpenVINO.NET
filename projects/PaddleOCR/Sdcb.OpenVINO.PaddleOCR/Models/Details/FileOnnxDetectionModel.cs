using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// File-based ONNX detection model.
/// </summary>
public class FileOnnxDetectionModel : DetectionModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileOnnxDetectionModel"/> class.
    /// </summary>
    public FileOnnxDetectionModel(string directoryPath, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
    }

    /// <summary>
    /// Gets the directory path containing the model files.
    /// </summary>
    public string DirectoryPath { get; }

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => core.ReadModel(Path.Combine(DirectoryPath, "inference.onnx"));
}
