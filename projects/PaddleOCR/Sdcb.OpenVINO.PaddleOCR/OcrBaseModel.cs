using Sdcb.OpenVINO.PaddleOCR.Models;
using System;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents an abstract base class for OCR models.
/// </summary>
/// <remarks>
/// The class serves as a base for optical character recognition(OCR) models.
/// </remarks>
public abstract class OcrBaseModel : BaseModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="OcrBaseModel"/> class.
    /// </summary>
    /// <param name="version">The version of model.</param>
    public OcrBaseModel(ModelVersion version)
    {
        Version = version;
    }

    /// <summary>
    /// Gets the version of the OCR model.
    /// </summary>
    public ModelVersion Version { get; }
}
