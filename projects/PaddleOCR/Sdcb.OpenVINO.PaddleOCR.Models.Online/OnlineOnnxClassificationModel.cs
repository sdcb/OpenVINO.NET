using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online ONNX text line orientation classification model.
/// </summary>
public record OnlineOnnxClassificationModel(string Name, Uri Uri, ModelVersion Version, NCHW Shape) : IOnlineClassificationModel
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts the model asynchronously.
    /// </summary>
    public async Task<FileOnnxClassificationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken, "inference.onnx", "inference.yml");
        return new FileOnnxClassificationModel(RootDirectory, Version, Shape);
    }

    async Task<ClassificationModel> IOnlineClassificationModel.DownloadAsync(CancellationToken cancellationToken)
    {
        return await DownloadAsync(cancellationToken);
    }

    /// <summary>
    /// PP-LCNet x0.25 text line orientation model.
    /// </summary>
    public static OnlineOnnxClassificationModel TextLineOrientationX025 => new("PP-LCNet_x0_25_textline_ori_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-LCNet_x0_25_textline_ori_onnx_infer.tar"), ModelVersion.V6, new NCHW(-1, 3, 80, 160));

    /// <summary>
    /// PP-LCNet x1.0 text line orientation model.
    /// </summary>
    public static OnlineOnnxClassificationModel TextLineOrientationX10 => new("PP-LCNet_x1_0_textline_ori_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-LCNet_x1_0_textline_ori_onnx_infer.tar"), ModelVersion.V6, new NCHW(-1, 3, 80, 160));
}
