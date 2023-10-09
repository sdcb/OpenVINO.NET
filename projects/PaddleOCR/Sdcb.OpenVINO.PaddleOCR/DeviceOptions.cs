using System;
using System.Collections.Generic;

namespace Sdcb.OpenVINO.PaddleOCR;

/// <summary>
/// Represents a collection of options for a device.
/// </summary>
public record DeviceOptions(string DeviceName = "CPU")
{
    /// <summary>
    /// Gets or sets a function to create a new OVCore instance.
    /// </summary>
    public Func<OVCore> CreateCore { get; init; } = () => SharedOVCore.Instance;

    /// <summary>
    /// Gets or sets a dictionary of properties to configure the CompiledModel.
    /// </summary>
    public Dictionary<string, string>? Properties { get; init; } = null;
}
