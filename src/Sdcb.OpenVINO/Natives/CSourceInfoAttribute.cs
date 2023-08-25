using System;

namespace Sdcb.OpenVINO.Natives;

public class CSourceInfoAttribute : Attribute
{
    public CSourceInfoAttribute(string headerFile, int lineNumberStart, int lineNumberEnd)
    {
        HeaderFile = headerFile;
        LineNumberStart = lineNumberStart;
        LineNumberEnd = lineNumberEnd;
    }

    public string HeaderFile { get; }

    public int LineNumberStart { get; }
    
    public int LineNumberEnd { get; }
}
