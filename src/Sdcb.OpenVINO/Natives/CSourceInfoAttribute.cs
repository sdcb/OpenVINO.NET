using System;

namespace Sdcb.OpenVINO.Natives;

/// <summary>
/// Class for handling C source file information.
/// </summary>
/// <remarks>
/// This attribute is used to specify information about the C source file and the corresponding line numbers of the function definition 
/// of an OpenVINO primitive layer implemented on the C side, which will be needed for mapping errors and debugging.
/// </remarks>
public class CSourceInfoAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CSourceInfoAttribute"/> class.
    /// </summary>
    /// <param name="headerFileName">The openvino C library header file name.</param>
    /// <param name="lineNumberStart">The first line number of the function definition.</param>
    /// <param name="lineNumberEnd">The last line number of the function definition.</param>
    /// <param name="group">The group name of the unit.</param>
    public CSourceInfoAttribute(string headerFileName, int lineNumberStart, int lineNumberEnd, string? group = null)
    {
        HeaderFileName = headerFileName;
        LineNumberStart = lineNumberStart;
        LineNumberEnd = lineNumberEnd;
        Group = group;
    }

    /// <summary>The openvino C library header file name.</summary>
    public string HeaderFileName { get; }

    /// <summary>The first line number of the function definition.</summary>
    public int LineNumberStart { get; }

    /// <summary>The last line number of the function definition.</summary>
    public int LineNumberEnd { get; }

    /// <summary>The group name of the unit.</summary>
    public string? Group { get; }
}
