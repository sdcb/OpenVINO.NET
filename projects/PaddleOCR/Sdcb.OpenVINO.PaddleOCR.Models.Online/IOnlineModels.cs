using Sdcb.OpenVINO.PaddleOCR.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Online;

/// <summary>
/// Represents an online detection model.
/// </summary>
public interface IOnlineDetectionModel
{
    /// <summary>
    /// Downloads the model asynchronously.
    /// </summary>
    Task<DetectionModel> DownloadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an online text line orientation classification model.
/// </summary>
public interface IOnlineClassificationModel
{
    /// <summary>
    /// Downloads the model asynchronously.
    /// </summary>
    Task<ClassificationModel> DownloadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an online recognition model.
/// </summary>
public interface IOnlineRecognizationModel
{
    /// <summary>
    /// Downloads the model asynchronously.
    /// </summary>
    Task<RecognizationModel> DownloadAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an online document orientation classification model.
/// </summary>
public interface IOnlineDocumentOrientationClassificationModel
{
    /// <summary>
    /// Downloads the model asynchronously.
    /// </summary>
    Task<DocumentOrientationClassificationModel> DownloadAsync(CancellationToken cancellationToken = default);
}
