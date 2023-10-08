// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Linq/src/System/Linq/FileDetectionModel.cs

using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR.Models.Details;

/// <summary>
/// Represents a model used for file detection using PaddleInference.
/// </summary>
public class FileDetectionModel : DetectionModel
{
    /// <summary>
    /// Gets the directory path containing the model files.
    /// </summary>
    public string DirectoryPath { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDetectionModel"/> class with the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The directory path containing the model files.</param>
    /// <param name="version">The version of detection model.</param>
    public FileDetectionModel(string directoryPath, ModelVersion version) : base(version)
    {
        DirectoryPath = directoryPath;
    }

    /// <inheritdoc/>
    public override Model CreateOVModel(OVCore core) => ReadDirectoryInferenceModel(core, DirectoryPath);
}
