using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online recognition model.
/// </summary>
public abstract record OnlineRecognizationModel(string Name, Uri Uri, ModelVersion Version)
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts a recognition model asynchronously.
    /// </summary>
    public abstract Task<RecognizationModel> DownloadAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// PP-OCRv6 medium ONNX recognition model.
    /// </summary>
    public static OnlineRecognizationModel ChineseV6Medium => new V6OnlineRecModel("PP-OCRv6_medium_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_medium_rec_onnx_infer.tar"));

    /// <summary>
    /// PP-OCRv6 small ONNX recognition model.
    /// </summary>
    public static OnlineRecognizationModel ChineseV6Small => new V6OnlineRecModel("PP-OCRv6_small_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_small_rec_onnx_infer.tar"));

    /// <summary>
    /// PP-OCRv6 tiny ONNX recognition model.
    /// </summary>
    public static OnlineRecognizationModel ChineseV6Tiny => new V6OnlineRecModel("PP-OCRv6_tiny_rec_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_tiny_rec_onnx_infer.tar"));

    /// <summary>
    /// Gets all available V6 online recognition models.
    /// </summary>
    public static OnlineRecognizationModel[] V6All => new[]
    {
        ChineseV6Medium,
        ChineseV6Small,
        ChineseV6Tiny,
    };
}

internal record V6OnlineRecModel(string Name, Uri Uri) : OnlineRecognizationModel(Name, Uri, ModelVersion.V6)
{
    public override async Task<RecognizationModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken, "inference.onnx", "inference.yml");
        return new FileOnnxRecognizationModel(RootDirectory, Version);
    }
}
