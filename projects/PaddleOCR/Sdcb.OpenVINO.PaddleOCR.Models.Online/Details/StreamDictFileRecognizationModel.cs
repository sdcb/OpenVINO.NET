using System.Collections.Generic;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;

internal class StreamDictFileRecognizationModel : RecognizationModel
{
    private readonly IReadOnlyList<string> _labels;

    public string DirectoryPath { get; }

    public StreamDictFileRecognizationModel(string directoryPath, IReadOnlyList<string> dict, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
        _labels = dict;
    }

    public override Model CreateOVModel(OVCore core) => core.ReadDirectoryPaddleModel(DirectoryPath);

    public override string GetLabelByIndex(int i) => GetLabelByIndex(i, _labels);
}
