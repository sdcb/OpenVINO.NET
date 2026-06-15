using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using Sdcb.OpenVINO.PaddleOCR.Models.Online.Details;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents a model for online object detection.
/// </summary>
public record OnlineDetectionModel(string Name, Uri Uri, ModelVersion Version)
{
    /// <summary>
    /// Gets the root directory of the model.
    /// </summary>
    public string RootDirectory => Path.Combine(Settings.GlobalModelDirectory, Name);

    /// <summary>
    /// Downloads and extracts the model files to the root directory asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A <see cref="DetectionModel"/> representing the downloaded and extracted file model.</returns>
    public async Task<DetectionModel> DownloadAsync(CancellationToken cancellationToken = default)
    {
        if (Version == ModelVersion.V6)
        {
            await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken, "inference.onnx");
            return new FileOnnxDetectionModel(RootDirectory, Version);
        }

        await Utils.DownloadAndExtractAsync(Name, Uri, RootDirectory, cancellationToken);
        return new FileDetectionModel(RootDirectory, Version);
    }

    /// <summary>
    /// PP-OCRv6 medium ONNX detection model.
    /// </summary>
    public static OnlineDetectionModel ChineseV6Medium => new("PP-OCRv6_medium_det_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_medium_det_onnx_infer.tar"), ModelVersion.V6);

    /// <summary>
    /// PP-OCRv6 small ONNX detection model.
    /// </summary>
    public static OnlineDetectionModel ChineseV6Small => new("PP-OCRv6_small_det_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_small_det_onnx_infer.tar"), ModelVersion.V6);

    /// <summary>
    /// PP-OCRv6 tiny ONNX detection model.
    /// </summary>
    public static OnlineDetectionModel ChineseV6Tiny => new("PP-OCRv6_tiny_det_onnx_infer", new Uri("https://paddle-model-ecology.bj.bcebos.com/paddlex/official_inference_model/paddle3.0.0/PP-OCRv6_tiny_det_onnx_infer.tar"), ModelVersion.V6);

    /// <summary>
    /// [New] v4 model, supporting Chinese, English, multilingual text detection
    /// (Size: 4.66M)
    /// </summary>
    public static OnlineDetectionModel ChineseV4 => new("ch_PP-OCRv4_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/chinese/ch_PP-OCRv4_det_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// [New] v4 server model, supporting Chinese, English, multilingual text detection
    /// (Size: 110M)
    /// </summary>
    public static OnlineDetectionModel ChineseServerV4 => new("detv4_teacher_inference", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv4/chinese/ch_PP-OCRv4_det_server_infer.tar"), ModelVersion.V4);

    /// <summary>
    /// slim quantization with distillation lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 1.1M)
    /// </summary>
    public static OnlineDetectionModel ChineseV3Slim => new("ch_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 3.8M)
    /// </summary>
    public static OnlineDetectionModel ChineseV3 => new("ch_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/chinese/ch_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// slim quantization with distillation lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 3M)
    /// </summary>
    public static OnlineDetectionModel ChineseV2Slim => new("ch_PP-OCRv2_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_slim_quant_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 3M)
    /// </summary>
    public static OnlineDetectionModel ChineseV2 => new("ch_PP-OCRv2_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv2/chinese/ch_PP-OCRv2_det_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Slim pruned lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 2.6M)
    /// </summary>
    public static OnlineDetectionModel ChineseMobileSlimV2 => new("ch_ppocr_mobile_slim_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/slim/ch_ppocr_mobile_v2.0_det_prune_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Original lightweight model, supporting Chinese, English, multilingual text detection
    /// (Size: 3M)
    /// </summary>
    public static OnlineDetectionModel ChineseMobileV2 => new("ch_ppocr_mobile_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_mobile_v2.0_det_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// General model, which is larger than the lightweight model, but achieved better performance
    /// (Size: 47M)
    /// </summary>
    public static OnlineDetectionModel ChineseServerV2 => new("ch_ppocr_server_v2.0_det", new Uri("https://paddleocr.bj.bcebos.com/dygraph_v2.0/ch/ch_ppocr_server_v2.0_det_infer.tar"), ModelVersion.V2);

    /// <summary>
    /// Slim qunatization with distillation lightweight detection model, supporting English
    /// (Size: 1.1M)
    /// </summary>
    public static OnlineDetectionModel EnglishV3Slim => new("en_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Original lightweight detection model, supporting English
    /// (Size: 3.8M)
    /// </summary>
    public static OnlineDetectionModel EnglishV3 => new("en_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/english/en_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Slim qunatization with distillation lightweight detection model, supporting English
    /// (Size: 1.1M)
    /// </summary>
    public static OnlineDetectionModel MultiLanguageV3Slim => new("ml_PP-OCRv3_det_slim", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/Multilingual_PP-OCRv3_det_slim_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Original lightweight detection model, supporting English
    /// (Size: 3.8M)
    /// </summary>
    public static OnlineDetectionModel MultiLanguageV3 => new("ml_PP-OCRv3_det", new Uri("https://paddleocr.bj.bcebos.com/PP-OCRv3/multilingual/Multilingual_PP-OCRv3_det_infer.tar"), ModelVersion.V3);

    /// <summary>
    /// Gets all available online detection models
    /// </summary>
    public static OnlineDetectionModel[] All => new[]
    {
        ChineseV6Medium,
        ChineseV6Small,
        ChineseV6Tiny,
        ChineseV4,
        ChineseServerV4,
        ChineseV3Slim,
        ChineseV3,
        ChineseV2Slim,
        ChineseV2,
        ChineseMobileSlimV2,
        ChineseMobileV2,
        ChineseServerV2,
        EnglishV3Slim,
        EnglishV3,
        MultiLanguageV3Slim,
        MultiLanguageV3,
    };
}
