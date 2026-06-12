using System.Collections.Generic;
using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// File-based ONNX recognition model whose labels are stored in inference.yml.
/// </summary>
public class FileOnnxRecognizationModel : RecognizationModel
{
    private readonly IReadOnlyList<string> _labels;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileOnnxRecognizationModel"/> class.
    /// </summary>
    public FileOnnxRecognizationModel(string directoryPath, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
        _labels = InferenceYmlUtils.LoadCharacterDict(Path.Combine(directoryPath, "inference.yml"));
    }

    /// <summary>
    /// Gets the directory path containing the model files.
    /// </summary>
    public string DirectoryPath { get; }

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => core.ReadModel(Path.Combine(DirectoryPath, "inference.onnx"));

    /// <inheritdoc/>
    public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
}
