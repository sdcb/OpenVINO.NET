using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using System;

namespace Sdcb.OpenVINO.PaddleOCR.Models;

/// <summary>
/// A base class for classification models used in PaddleOCR. Provides methods to create 
/// a PaddleConfig object and OcrShape object, as well as a static method to create a
/// ClassificationModel object from a directory path. 
/// </summary>
public abstract class ClassificationModel : OcrBaseModel
{
    /// <summary>
    /// Gets the OcrShape of this classification model.
    /// </summary>
    public abstract NCHW Shape { get; }

    /// <summary>
    /// Gets the preprocessing mode for this classification model.
    /// </summary>
    public virtual ClassificationPreprocessMode PreprocessMode => ClassificationPreprocessMode.Legacy;

    /// <summary>
    /// Gets the resize mode for this classification model.
    /// </summary>
    public virtual ClassificationResizeMode ResizeMode => ClassificationResizeMode.ResizeAndPad;

    /// <summary>
    /// The default OcrShape used in the classification model.
    /// </summary>
    public static NCHW DefaultShape = new(-1, 3, 48, 192);

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassificationModel"/> class with a given model version.
    /// </summary>
    /// <param name="version">The version of the classification model.</param>
    protected ClassificationModel(ModelVersion version) : base(version)
    {
    }

    /// <summary>
    /// Creates a ClassificationModel object from the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The path to the directory containing the model files</param>
    /// <param name="version">The version of the classification model.</param>
    /// <returns>a new ClassificationModel object</returns>
    public static ClassificationModel FromDirectory(string directoryPath, ModelVersion version = ModelVersion.V2) => new FileClassificationModel(directoryPath, version);

    /// <inheritdoc/>
    public override void AfterReadModel(Model model)
    {
        model.ReshapePrimaryInput(new PartialShape(Dimension.Dynamic, 3, Shape.Height, Shape.Width));
    }
}

/// <summary>
/// Defines preprocessing modes for text line orientation classifiers.
/// </summary>
public enum ClassificationPreprocessMode
{
    /// <summary>
    /// Uses the historical PaddleOCR classifier normalization.
    /// </summary>
    Legacy,

    /// <summary>
    /// Uses RGB ImageNet mean/std normalization.
    /// </summary>
    ImageNetRgb,
}

/// <summary>
/// Defines resize modes for text line orientation classifiers.
/// </summary>
public enum ClassificationResizeMode
{
    /// <summary>
    /// Preserves the historical PaddleOCR classifier resize behavior by cropping wide images and padding narrow images.
    /// </summary>
    ResizeAndPad,

    /// <summary>
    /// Resizes directly to the model input size.
    /// </summary>
    DirectResize,
}
