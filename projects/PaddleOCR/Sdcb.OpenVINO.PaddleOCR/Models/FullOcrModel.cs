namespace Sdcb.OpenVINO.PaddleOCR.Models;

/// <summary>
/// Represents a full OCR model composed of a detection, classification and recognition model.
/// </summary>
public class FullOcrModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="classificationModel">A classification model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    public FullOcrModel(DetectionModel detectionModel, ClassificationModel? classificationModel, RecognizationModel recognizationModel)
        : this(detectionModel, classificationModel, recognizationModel, documentOrientationModel: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="classificationModel">A text line orientation classification model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    /// <param name="documentOrientationModel">A document orientation classification model.</param>
    public FullOcrModel(
        DetectionModel detectionModel,
        ClassificationModel? classificationModel,
        RecognizationModel recognizationModel,
        DocumentOrientationClassificationModel? documentOrientationModel)
    {
        DetectionModel = detectionModel;
        ClassificationModel = classificationModel;
        RecognizationModel = recognizationModel;
        DocumentOrientationModel = documentOrientationModel;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    public FullOcrModel(DetectionModel detectionModel, RecognizationModel recognizationModel)
        : this(detectionModel, classificationModel: null, recognizationModel)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FullOcrModel"/> class.
    /// </summary>
    /// <param name="detectionModel">A detection model.</param>
    /// <param name="recognizationModel">A recognition model.</param>
    /// <param name="documentOrientationModel">A document orientation classification model.</param>
    public FullOcrModel(
        DetectionModel detectionModel,
        RecognizationModel recognizationModel,
        DocumentOrientationClassificationModel? documentOrientationModel)
        : this(detectionModel, classificationModel: null, recognizationModel, documentOrientationModel)
    {
    }

    /// <summary>
    /// Gets or sets the detection model.
    /// </summary>
    public DetectionModel DetectionModel { get; init; }

    /// <summary>
    /// Gets or sets the classification model.
    /// </summary>
    public ClassificationModel? ClassificationModel { get; init; }

    /// <summary>
    /// Gets or sets the recognition model.
    /// </summary>
    public RecognizationModel RecognizationModel { get; init; }

    /// <summary>
    /// Gets or sets the document orientation classification model.
    /// </summary>
    public DocumentOrientationClassificationModel? DocumentOrientationModel { get; init; }
}
