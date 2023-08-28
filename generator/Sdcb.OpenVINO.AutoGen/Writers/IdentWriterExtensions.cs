using System.CodeDom.Compiler;

namespace Sdcb.OpenVINO.AutoGen.Writers;

internal static class IdentWriterExtensions
{
    public static void BeginIdent(this IndentedTextWriter w, Action writer)
    {
        w.WriteLine("{");
        w.Indent++;
        writer();
        w.Indent--;
        w.WriteLine("}");
    }

    public static void WriteLines(this IndentedTextWriter w, IEnumerable<string> lines)
    {
        foreach (string line in lines)
        {
            w.WriteLine(line);
        }
    }
}
