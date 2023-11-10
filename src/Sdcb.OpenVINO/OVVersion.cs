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

    /// <summary>
    /// Gets the abbreviated version number for this <see cref="OVVersion"/> record.
    /// </summary>
    /// <returns>The abbreviated version number.</returns>
    /// <remarks>
    /// The abbreviated version number is calculated by taking the last two digits of the major version number, the last digit of the minor version number, and the last digit of the build number.
    /// </remarks>
    internal int GetAbbreviatedVersion()
    {
        Version ver = ToVersion();
        int major = ver.Major % 100;
        int minor = ver.Minor % 10;
        int build = ver.Build % 10;
        return major * 100 + minor * 10 + build;
    }

    internal Version ToVersion()
    {
        // Known buildNumbers:
        // * 2023.1.0-000--
        // * 2023.1.0-12185-9e6b00e51cd-releases/2023/1
        // * 2023.2.0-13098-49c8526e20f-releases/2023/2
        try
        {
            string[] parts = BuildNumber.Split(new[] { '.' }, StringSplitOptions.None);

            if (parts.Length < 3)
            {
                // Invalid version number
                Console.WriteLine($"Failed to parse {BuildNumber} in method {nameof(ToVersion)}.");
                return new Version(2023, 1, 0, 0);
            }

            int major = int.Parse(parts[0]);
            int minor = int.Parse(parts[1]);
            int build = int.Parse(parts[2].Split(new[] { '-' }, StringSplitOptions.None)[0]);

            return new Version(major, minor, build, 0);
        }
        catch (Exception)
        {
            // Log exception, if necessary
            Console.WriteLine($"Failed to parse {BuildNumber} in method {nameof(ToVersion)}.");
            return new Version(2023, 1, 0, 0);
        }
    }
}
