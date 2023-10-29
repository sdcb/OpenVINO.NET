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
    public PaddleOcrOptions(DeviceOptions allDeviceOptions, InferRequestQueueOptions? irQueueOptions = null)
    {
        DetectionDeviceOptions = allDeviceOptions;
        ClassificationDeviceOptions = allDeviceOptions;
        RecognitionDeviceOptions = allDeviceOptions;

        if (irQueueOptions != null)
        {
            DetectionIRQueueOptions = irQueueOptions;
            ClassificationIRQueueOptions = irQueueOptions;
            RecognitionIRQueueOptions = irQueueOptions;
        }

        if (allDeviceOptions.DeviceName == "GPU")
        {
            DetectionStaticSize = new Size(1024, 1024);
            RecognitionStaticWidth = 512;
        }
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

    public InferRequestQueueOptions? DetectionIRQueueOptions { get; set; }
    public InferRequestQueueOptions? ClassificationIRQueueOptions { get; set; }
    public InferRequestQueueOptions? RecognitionIRQueueOptions { get; set; }

    /// <summary>
    /// Get or set the static size for the detection model graph. 
    /// It may improve performance but could involve image scaling during inference, 
    /// possibly reducing accuracy. If `null`, the graph size is dynamic.
    /// </summary>
    public Size? DetectionStaticSize { get; set; }

    /// <summary>
    /// Get or set the fixed width for the OCR rec model input. 
    /// Increasing this value may improve performance, 
    /// but may also result in automatic image scaling during inference, 
    /// potentially reducing accuracy. If `null`, input shape is dynamic.
    /// </summary>
    public int? RecognitionStaticWidth { get; set; }
}
