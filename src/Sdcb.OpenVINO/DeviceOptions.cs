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
    /// Initializes a new instance of the <see cref="DeviceOptions"/> class with default values, where deviceName is set to <see cref="DefaultDeviceName"/>.
    /// </summary>
    /// <remarks>
    /// This constructor does not set any properties. Use the properties of the class to specify inference performance mode and increase the number of inference threads. 
    /// </remarks>
    /// <param name="deviceName">The name of the device to use.</param>
    [SetsRequiredMembers]
    public DeviceOptions(string deviceName = DefaultDeviceName) : this(deviceName, new())
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
    /// Gets or sets the number of inference streams.
    /// </summary>
    public NumStreamDef? NumStreams
    {
        get => Properties.TryGetValue(PropertyKeys.NumStreams, out string? val) ? NumStreamDef.Parse(val) : null;
        set
        {
            if (value != null)
            {
                Properties[PropertyKeys.NumStreams] = value.Value.ToString();
            }
            else
            {
                Properties.Remove(PropertyKeys.NumStreams);
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
    /// Creates a new <see cref="OVCore"/> instance.
    /// </summary>
    public virtual OVCore CreateOVCore() => OVCore.Shared;

    /// <summary>
    /// Gets or sets a dictionary of properties to configure the CompiledModel.
    /// </summary>
    public required Dictionary<string, string> Properties { get; init; }

    /// <summary>
    /// The default device name to be AUTO.
    /// </summary>
    public const string DefaultDeviceName = "AUTO";
}

public readonly record struct NumStreamDef(int StreamCount)
{
    public static NumStreamDef Auto => new(-1);
    public static NumStreamDef Numa => new(-2);
    public static implicit operator NumStreamDef(int streamCount) => streamCount switch
    {
        < -2 => throw new ArgumentException("value must >= -2 or AUTO/NUMA", nameof(streamCount)),
        _ => new NumStreamDef(streamCount),
    };

    public static NumStreamDef Parse(string streamCount) => streamCount switch
    {
        "AUTO" => Auto,
        "NUMA" => Numa,
        _ => int.Parse(streamCount)
    };

    public override string ToString() => StreamCount switch
    {
        -1 => "AUTO",
        -2 => "NUMA",
        _ => StreamCount.ToString()
    };
}