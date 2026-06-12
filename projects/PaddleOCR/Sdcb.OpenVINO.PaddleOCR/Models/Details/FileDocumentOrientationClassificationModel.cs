using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// File-based ONNX document orientation classification model.
/// </summary>
public class FileDocumentOrientationClassificationModel : DocumentOrientationClassificationModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FileDocumentOrientationClassificationModel"/> class.
    /// </summary>
    public FileDocumentOrientationClassificationModel(string directoryPath)
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
