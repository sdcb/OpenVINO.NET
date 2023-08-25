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

    public static void WriteLines(this IndentedTextWriter w, string text)
    {
        foreach (string line in text.Split(Environment.NewLine))
        {
            w.WriteLine(line);
        }
    }
}
