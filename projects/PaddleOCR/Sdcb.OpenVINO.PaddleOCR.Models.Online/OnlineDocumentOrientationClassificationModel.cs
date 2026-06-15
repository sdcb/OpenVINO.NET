using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online ONNX document orientation classification model.
/// </summary>
public record OnlineDocumentOrientationClassificationModel(string Name, Uri Uri)
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts the model asynchronously.
    /// </summary>
    public async Task<FileDocumentOrientationClassificationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken, "inference.onnx", "inference.yml");
        return new FileDocumentOrientationClassificationModel(RootDirectory);
    }

    /// <summary>
    /// PP-LCNet x1.0 document orientation model.
    /// </summary>
    public static OnlineDocumentOrientationClassificationModel PPDocOrientationX10 => new("PP-LCNet_x1_0_doc_ori_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-LCNet_x1_0_doc_ori_onnx_infer.tar"));
}
