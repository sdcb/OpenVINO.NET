using Sdcb.OpenVINO.Natives;
using System;

namespace Sdcb.OpenVINO;

/// <summary>
/// <see cref="OVVersion"/> is used to represent OpenVINO version with build number and description.
/// </summary>
/// <param name="BuildNumber">The build number for this version of OpenVINO.</param>
/// <param name="Description">The description of this version of OpenVINO.</param>
public record OVVersion(string BuildNumber, string Description)
{
    /// <summary>
    /// Converts an <see cref="ov_version"/> struct to an <see cref="OVVersion"/> record.
    /// </summary>
    /// <param name="ver">The <see cref="ov_version"/> struct to convert.</param>
    /// <returns>The converted <see cref="OVVersion"/> record.</returns>
    public static unsafe OVVersion FromNative(in ov_version ver)
    {
        string buildNumber = StringUtils.UTF8PtrToString((IntPtr)ver.buildNumber)!;
        string description = StringUtils.UTF8PtrToString((IntPtr)ver.description)!;
        return new OVVersion(buildNumber, description);
    }
}
