using Sdcb.OpenVINO.Natives;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

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

    private OpenVINOException(ov_status_e e, string message) : base(message)
    {
        Status = e;
    }

    /// <summary>
    /// Throws an exception if the given OpenVINO status is not OK.
    /// </summary>
    /// <param name="e">The OpenVINO status to check.</param>
    /// <param name="message">The error message to display (optional).</param>
    /// <param name="callerMemberName">The name of the calling member (optional).</param>
    /// <param name="callerExpression">The name of the calling expression (optional).</param>
    /// <param name="callerFilePath">The path of the calling file (optional).</param>
    /// <param name="callerLineNumber">The line number of the calling expression (optional).</param>
    /// <exception cref="OpenVINOException">Thrown when the provided OpenVINO status is not <see cref="ov_status_e.OK"/>.</exception>
    public static void ThrowIfFailed(ov_status_e e,
        string? message = null,
        [CallerMemberName] string? callerMemberName = null,
        [CallerArgumentExpression(nameof(e))] string? callerExpression = null,
        [CallerFilePath] string? callerFilePath = null,
        [CallerLineNumber] int? callerLineNumber = null)
    {
        if (e != ov_status_e.OK)
        {
            throw new OpenVINOException(e, OVStatusToString(e, message, callerMemberName, callerExpression, callerFilePath, callerLineNumber));
        }
    }

    /// <summary>
    /// Returns a string describing the specified OpenVINO error code.
    /// </summary>
    /// <param name="e">The OpenVINO status code to retrieve error information for.</param>
    public static unsafe string GetErrorInfo(ov_status_e e)
    {
        byte* errorInfoPtr = ov_get_error_info(e);
        return Marshal.PtrToStringAnsi((IntPtr)errorInfoPtr)!;
    }

    internal unsafe static string OVStatusToString(ov_status_e e, 
        string? message, string? callerMemberName, string? callerExpression, string? callFilePath, int? callerLineNumber)
    {
        StringBuilder sb = new();
        sb.Append($"{GetErrorInfo(e)}({(int)e})");
        if (message != null)
        {
            sb.Append($" {message}");
        }
        if (callFilePath != null)
        {
            sb.Append($" in file \"{callFilePath}\"");
        }
        if (callerMemberName != null)
        {
            sb.Append($" member \"{callerMemberName}\"");
        }
        if (callerLineNumber != null)
        {
            sb.Append($" at line {callerLineNumber}");
        }
        if (callerExpression != null)
        {
            sb.Append($" when calling \"{callerExpression}\"");
        }
        return sb.ToString();
    }
}