using Sdcb.OpenVINO.PaddleOCR.Models.Details;

namespace Sdcb.OpenVINO.PaddleOCR.Models;

/// <summary>
/// A base class for document orientation classification models.
/// </summary>
public abstract class DocumentOrientationClassificationModel : BaseModel
{
    /// <summary>
    /// Gets the model input shape.
    /// </summary>
    public virtual NCHW Shape => DefaultShape;

    /// <summary>
    /// The default document orientation model shape.
    /// </summary>
    public static NCHW DefaultShape { get; } = new(-1, 3, 224, 224);

    /// <summary>
    /// Creates a document orientation model object from the specified directory path.
    /// </summary>
    public static DocumentOrientationClassificationModel FromDirectory(string directoryPath) => new FileDocumentOrientationClassificationModel(directoryPath);
}
