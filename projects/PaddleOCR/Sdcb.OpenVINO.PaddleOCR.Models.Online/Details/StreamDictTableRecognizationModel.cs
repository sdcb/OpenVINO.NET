using System.Collections.Generic;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;

internal class StreamDictTableRecognizationModel : TableRecognitionModel
{
    public string DirectoryPath { get; }

    public StreamDictTableRecognizationModel(string directoryPath, IReadOnlyList<string> dict) : base(dict)
    {
        DirectoryPath = directoryPath;
    }

    public override Model CreateOVModel(OVCore core) => core.ReadDirectoryPaddleModel(DirectoryPath);
}
