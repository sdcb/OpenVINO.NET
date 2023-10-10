using System.IO;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Provides extension methods for reading Paddle model in OpenVINO.
/// </summary>
public static class PaddleOVCoreExtensions
{
    /// <summary>
    /// Reads the inference model from a directory path and returns the <see cref="Model"/> instance.
    /// </summary>
    /// <remarks>
    /// The purpose of this method is to read the inference model from the directory path and return the pre-trained inference model wrapped in a <see cref="Model"/> object.
    /// </remarks>
    /// <param name="core">The OpenVINO Core used for reading the model.</param>
    /// <param name="directoryPath">The directory path where the inference model is located.</param>
    /// <param name="modelFileName">The file name of the pre-trained model file, by default "inference.pdmodel".</param>
    /// <returns>Returns a <see cref="Model"/> instance representing the pre-trained inference model.</returns>
    public static Model ReadDirectoryPaddleModel(this OVCore core, string directoryPath, string modelFileName = "inference.pdmodel")
    {
        return core.ReadModel(Path.Combine(directoryPath, modelFileName));
    }
}
