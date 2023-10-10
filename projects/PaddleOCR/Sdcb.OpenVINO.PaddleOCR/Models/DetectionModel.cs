using Sdcb.OpenVINO.PaddleOCR.Models.Details;
using System;
using System.Collections.Generic;

namespace Sdcb.OpenVINO.PaddleOCR.Models;

/// <summary>
/// Represents a detection model.
/// </summary>
public abstract class DetectionModel : OcrBaseModel
{
    /// <summary>
    /// Constructor for initializing an instance of the <see cref="DetectionModel"/> class.
    /// </summary>
    /// <param name="version">The version of detection model.</param>
    public DetectionModel(ModelVersion version) : base(version)
    {
    }

    /// <summary>
    /// Returns an instance of the DetectionModel class from the directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path where model files are located.</param>
    /// <param name="version">The version of detection model.</param>
    /// <returns>An instance of the DetectionModel class.</returns>
    public static DetectionModel FromDirectory(string directoryPath, ModelVersion version) => new FileDetectionModel(directoryPath, version);
}
