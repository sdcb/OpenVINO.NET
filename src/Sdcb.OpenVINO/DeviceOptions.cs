using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sdcb.OpenVINO;

/// <summary>
/// Represents a collection of options for a device.
/// </summary>
/// <remarks>
/// This class represents a collection of options that can be used to configure a device. 
/// The properties of the class can be used to specify inference performance mode and increase the number of inference threads. 
/// </remarks>
public record DeviceOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceOptions"/> class with default values, where deviceName is set to "CPU".
    /// </summary>
    /// <remarks>
    /// This constructor does not set any properties. Use the properties of the class to specify inference performance mode and increase the number of inference threads. 
    /// </remarks>
    /// <param name="deviceName">The name of the device to use.</param>
    [SetsRequiredMembers]
    public DeviceOptions(string deviceName = "CPU") : this(deviceName, new())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceOptions"/> class with the specified device name and properties.
    /// </summary>
    /// <remarks>
    /// Use the properties of the class to specify inference performance mode and increase the number of inference threads. 
    /// </remarks>
    /// <param name="deviceName">The name of the device to use.</param>
    /// <param name="properties">A dictionary of properties to configure the device.</param>
    [SetsRequiredMembers]
    public DeviceOptions(string deviceName, Dictionary<string, string> properties)
    {
        DeviceName = deviceName;
        Properties = properties;
    }

    /// <summary>
    /// Gets or sets the name of the device.
    /// </summary>
    public required string DeviceName { get; init; }

    /// <summary>
    /// Gets or sets the number of inference threads. 
    /// </summary>
    public int? InferenceNumThreads
    {
        get => Properties.TryGetValue(PropertyKeys.InferenceNumThreads, out string? val) ? int.Parse(val) : null;
        set
        {
            if (value != null)
            {
                Properties[PropertyKeys.InferenceNumThreads] = value.Value.ToString();
            }
            else
            {
                Properties.Remove(PropertyKeys.InferenceNumThreads);
            }
        }
    }

    /// <summary>
    /// Gets or sets the inference performance mode. 
    /// </summary>
    public PerformanceMode? PerformanceMode
    {
        get => Properties.TryGetValue(PropertyKeys.HintPerformanceMode, out string? val) ? (PerformanceMode)Enum.Parse(typeof(PerformanceMode), val) : null;
        set
        {
            if (value != null)
            {
                Properties[PropertyKeys.HintPerformanceMode] = value.Value switch
                {
                    OpenVINO.PerformanceMode.Latency => "LATENCY",
                    OpenVINO.PerformanceMode.Throughput => "THROUGHPUT",
                    OpenVINO.PerformanceMode.CumulativeThroughput => "CUMULATIVE_THROUGHPUT",
                    _ => throw new ArgumentOutOfRangeException(nameof(PerformanceMode)),
                };
            }
            else
            {
                Properties.Remove(PropertyKeys.HintPerformanceMode);
            }
        }
    }

    /// <summary>
    /// Gets or sets a function to create a new <see cref="OVCore"/> instance.
    /// </summary>
    public Func<OVCore> CreateOVCore { get; init; } = () => SharedOVCore.Instance;

    /// <summary>
    /// Gets or sets a dictionary of properties to configure the CompiledModel.
    /// </summary>
    public required Dictionary<string, string> Properties { get; init; }
}
