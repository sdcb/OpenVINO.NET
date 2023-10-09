using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// This class represents a model used for table recognition from files.
/// </summary>
public class FileTableRecognizationModel : TableRecognitionModel
{
    /// <summary>
    /// The directory path of the model.
    /// </summary>
    public string DirectoryPath { get; }
    /// <summary>
    /// The path of the label file used by the model.
    /// </summary>
    public string LabelFilePath { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTableRecognizationModel"/> class with the directory path and label file path.
    /// </summary>
    /// <param name="directoryPath">The directory path of the model.</param>
    /// <param name="labelFilePath">The path of the label file used by the model.</param>
    public FileTableRecognizationModel(string directoryPath, string labelFilePath) : base(File.ReadAllLines(labelFilePath))
    {
        DirectoryPath = directoryPath;
        LabelFilePath = labelFilePath;
    }

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => ReadDirectoryInferenceModel(core, DirectoryPath);
}
