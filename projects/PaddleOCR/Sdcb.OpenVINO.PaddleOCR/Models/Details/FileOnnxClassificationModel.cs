using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// File-based ONNX text line orientation classification model.
/// </summary>
public class FileOnnxClassificationModel : ClassificationModel
{
    /// <summary>
    /// The default input shape for PP-LCNet text line orientation ONNX models.
    /// </summary>
    public static NCHW DefaultTextLineOrientationShape { get; } = new(-1, 3, 80, 160);

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOnnxClassificationModel"/> class.
    /// </summary>
    public FileOnnxClassificationModel(string directoryPath, ModelVersion version)
        : this(directoryPath, version, DefaultTextLineOrientationShape)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOnnxClassificationModel"/> class.
    /// </summary>
    public FileOnnxClassificationModel(
        string directoryPath,
        ModelVersion version,
        NCHW shape,
        ClassificationPreprocessMode preprocessMode = ClassificationPreprocessMode.ImageNetRgb,
        ClassificationResizeMode resizeMode = ClassificationResizeMode.DirectResize) : base(version)
    {
        DirectoryPath = directoryPath;
        Shape = shape;
        PreprocessMode = preprocessMode;
        ResizeMode = resizeMode;
    }

    /// <summary>
    /// Gets the directory path containing the model files.
    /// </summary>
    public string DirectoryPath { get; }

    /// <inheritdoc/>
    public override NCHW Shape { get; }

    /// <inheritdoc/>
    public override ClassificationPreprocessMode PreprocessMode { get; }

    /// <inheritdoc/>
    public override ClassificationResizeMode ResizeMode { get; }

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => core.ReadModel(Path.Combine(DirectoryPath, "inference.onnx"));
}
