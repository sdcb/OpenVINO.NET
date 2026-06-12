using Sdcb.OpenVINO.PaddleOCR.Models;
using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online ONNX recognition model.
/// </summary>
public record OnlineOnnxRecognizationModel(string Name, Uri Uri, ModelVersion Version) : IOnlineRecognizationModel
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts the model files to the root directory asynchronously.
    /// </summary>
    public async Task<FileOnnxRecognizationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken, "inference.onnx", "inference.yml");
        return new FileOnnxRecognizationModel(RootDirectory, Version);
    }

    async Task<RecognizationModel> IOnlineRecognizationModel.DownloadAsync(CancellationToken cancellationToken)
    {
        return await DownloadAsync(cancellationToken);
    }

    /// <summary>
    /// PP-OCRv6 medium recognition model.
    /// </summary>
    public static OnlineOnnxRecognizationModel ChineseV6Medium => new("PP-OCRv6_medium_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_medium_rec_onnx_infer.tar"), ModelVersion.V6);

    /// <summary>
    /// PP-OCRv6 small recognition model.
    /// </summary>
    public static OnlineOnnxRecognizationModel ChineseV6Small => new("PP-OCRv6_small_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_small_rec_onnx_infer.tar"), ModelVersion.V6);

    /// <summary>
    /// PP-OCRv6 tiny recognition model.
    /// </summary>
    public static OnlineOnnxRecognizationModel ChineseV6Tiny => new("PP-OCRv6_tiny_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_tiny_rec_onnx_infer.tar"), ModelVersion.V6);
}
