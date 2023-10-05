using Sdcb.OpenVINO.Natives;
using System;
using System.Runtime.InteropServices;

namespace Sdcb.OpenVINO;

using static NativeMethods;

/// <summary>
/// Represents errors that occur during OpenVINO operations.
/// </summary>
public class OpenVINOException : Exception
{
    /// <summary>
    /// Gets the OpenVINO status associated with the current exception.
    /// </summary>
    public ov_status_e Status { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenVINOException"/> class with a specified OpenVINO status.
    /// </summary>
    /// <param name="e">The OpenVINO status which caused the exception.</param>
    public OpenVINOException(ov_status_e e) : base(OVStatusToString(e))
    {
        Status = e;
    }

    /// <summary>
    /// Throws an exception if the given OpenVINO status is not OK.
    /// </summary>
    /// <param name="e">The OpenVINO status to check.</param>
    /// <exception cref="OpenVINOException">Thrown when the provided OpenVINO status is not <see cref="ov_status_e.OK"/>.</exception>
    public static void ThrowIfFailed(ov_status_e e)
    {
        if (e != ov_status_e.OK)
        {
            throw new OpenVINOException(e);
        }
    }

    internal unsafe static string? OVStatusToString(ov_status_e e)
    {
        byte* errorInfoPtr = ov_get_error_info(e);
        string? errorInfo = Marshal.PtrToStringAnsi((IntPtr)errorInfoPtr);
        return $"{errorInfo}({(int)e})";
    }
}