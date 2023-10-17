using OpenCvSharp;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// A class representing options for PaddleOCR.
/// </summary>
public class PaddleOcrOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrOptions"/> class.
    /// </summary>
    public PaddleOcrOptions() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaddleOcrOptions"/> class with specified device options.
    /// </summary>
    /// <param name="allDeviceOptions">The device options for detection, classification and recognition.</param>
    public PaddleOcrOptions(DeviceOptions allDeviceOptions)
    {
        DetectionDeviceOptions = allDeviceOptions;
        ClassificationDeviceOptions = allDeviceOptions;
        RecognitionDeviceOptions = allDeviceOptions;
    }

    /// <summary>
    /// Gets or sets the device options for detection.
    /// The DetectionStaticSize property can be used to specify a specific size as model graph size (instead of dynamic graph).
    /// </summary>
    public DeviceOptions? DetectionDeviceOptions { get; set; }

    /// <summary>
    /// Gets or sets the device options for classification.
    /// </summary>
    public DeviceOptions? ClassificationDeviceOptions { get; set; }

    /// <summary>
    /// Gets or sets the device options for recognition.
    /// </summary>
    public DeviceOptions? RecognitionDeviceOptions { get; set; }

    /// <summary>
    /// Gets or sets the specific size as model graph size (instead of dynamic graph) for detection model.
    /// </summary>
    public Size? DetectionStaticSize { get; set; }
}
