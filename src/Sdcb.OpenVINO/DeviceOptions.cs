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
    public NumStreamsDef? NumStreams
    {
        get => Properties.TryGetValue(PropertyKeys.NumStreams, out string? val) ? NumStreamsDef.Parse(val) : null;
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
    public PerformanceMode PerformanceMode
    {
        get => Properties.TryGetValue(PropertyKeys.HintPerformanceMode, out string? val) ? val switch
        {
            "LATENCY" => PerformanceMode.Latency,
            "THROUGHPUT" => PerformanceMode.Throughput,
            "CUMULATIVE_THROUGHPUT" => PerformanceMode.CumulativeThroughput,
            _ => throw new NotSupportedException($"{nameof(PerformanceMode)} {val} is not supported.")
        } : PerformanceMode.Latency;
        set
        {
            Properties[PropertyKeys.HintPerformanceMode] = value switch
            {
                PerformanceMode.Latency => "LATENCY",
                PerformanceMode.Throughput => "THROUGHPUT",
                PerformanceMode.CumulativeThroughput => "CUMULATIVE_THROUGHPUT",
                _ => throw new ArgumentOutOfRangeException(nameof(PerformanceMode)),
            };
        }
    }

    /// <summary>
    /// Core type can be used for CPU tasks on different devices
    /// </summary>
    public SchedulingCoreType SchedulingCoreType
    {
        get => Properties.TryGetValue(PropertyKeys.HintSchedulingCoreType, out string? val) ? val switch
        {
            "ANY_CORE" => SchedulingCoreType.AnyCore,
            "PCORE_ONLY" => SchedulingCoreType.PCoresOnly,
            "ECORE_ONLY" => SchedulingCoreType.ECoresOnly,
            _ => throw new NotSupportedException($"{nameof(SchedulingCoreType)} {val} is not supported.")
        } : SchedulingCoreType.AnyCore;
        set
        {
            Properties[PropertyKeys.HintSchedulingCoreType] = value switch
            {
                SchedulingCoreType.AnyCore => "ANY_CORE",
                SchedulingCoreType.PCoresOnly => "PCORE_ONLY",
                SchedulingCoreType.ECoresOnly => "ECORE_ONLY",
                _ => throw new ArgumentOutOfRangeException(nameof(SchedulingCoreType)),
            };
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether hyper-threading is enabled for OpenVINO inference.
    /// </summary>
    /// <remarks>
    /// This property controls whether hyper-threading is enabled for OpenVINO inference.
    /// By default, hyper-threading is disabled.
    /// Enabling hyper-threading may not provide significant performance benefits, as the CPU's floating-point units are shared between hyper-threaded cores.
    /// </remarks>
    public bool EnableHyperThreading
    {
        get => Properties.TryGetValue(PropertyKeys.HintEnableHyperThreading, out string? val) && val switch 
        { 
            "YES" => true, 
            "NO" => false, 
            _ => throw new NotSupportedException($"{nameof(EnableHyperThreading)} {val} is not supported.") 
        };
        set
        {
            if (value)
            {
                Properties[PropertyKeys.HintEnableHyperThreading] = value switch
                {
                    true => "YES",
                    _ => "NO"
                };
            }
            else
            {
                Properties.Remove(PropertyKeys.HintEnableHyperThreading);
            }
        }
    }

    /// <summary>
    /// Creates a new <see cref="OVCore"/> instance.
    /// </summary>
    public Func<OVCore> CreateOVCore { get; set; } = DefaultCreateOVCore;

    private static OVCore DefaultCreateOVCore() => OVCore.Shared;

    /// <summary>
    /// Gets or sets a dictionary of properties to configure the CompiledModel.
    /// </summary>
    public required Dictionary<string, string> Properties { get; init; }

    /// <summary>
    /// The default device name to be AUTO.
    /// </summary>
    public const string DefaultDeviceName = "AUTO";
}
