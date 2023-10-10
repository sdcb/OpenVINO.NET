using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// File classification model.
/// </summary>
public class FileClassificationModel : ClassificationModel
{
    /// <summary>
    /// Gets or sets the directory path for the model.
    /// </summary>
    public string DirectoryPath { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileClassificationModel"/> class.
    /// </summary>
    /// <param name="directoryPath">The directory path for the model.</param>
    /// <param name="version">The version of the classification model.</param>
    public FileClassificationModel(string directoryPath, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
    }

    /// <inheritdoc/>
    public override NCHW Shape => DefaultShape;

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => core.ReadDirectoryPaddleModel(DirectoryPath);
}
