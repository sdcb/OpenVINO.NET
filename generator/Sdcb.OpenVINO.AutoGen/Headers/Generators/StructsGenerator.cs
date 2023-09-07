using CppSharp.AST;
using Sdcb.OpenVINO.AutoGen.Writers;
using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

public class StructsGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Class[] structs = info.Units
                    .SelectMany(x => x.Classes)
                    .OrderBy(x => ((TranslationUnit)x.OriginalNamespace).FileName)
                    .ThenBy(x => x.Name)
                    .ToArray();
        Console.WriteLine($"Detected structs: {structs.Length}");
        return new(structs.Select(TransformOne));
    }

    private static GeneratedUnit TransformOne(Class @class)
    {
        IndentedLinesWriter w = new();
        DoxygenTags tags = DoxygenTags.Parse(@class.Comment?.Text);
        w.WriteLines(tags.ToBriefComment());

        VerbatimLineComment? groupBlock = @class.Comment?.FullComment.Blocks
            .OfType<VerbatimLineComment>()
            .Where(x => x.CommandKind == CommentCommandKind.A && x.Text.Trim() != @class.Name)
            .LastOrDefault();
        string? group = groupBlock?.Text.Trim();
        string headerFile = ((TranslationUnit)@class.OriginalNamespace).FileName;

        w.WriteLine($"[StructLayout(LayoutKind.Sequential), CSourceInfo(\"{headerFile}\", {@class.LineNumberStart}, {@class.LineNumberEnd}, \"{group}\")]");

        string unsafeBlock = @class.Fields.Any(x => CSharpUtils.TypeTransform(x.Type).Contains('*')) ? "unsafe " : "";
        w.WriteLine($"public {unsafeBlock}struct {@class.Name}");
        using (w.BeginIdent())
        {
            for (int i = 0; i < @class.Fields.Count; i++)
            {
                Field field = @class.Fields[i];
                DoxygenTags itemTags = DoxygenTags.Parse(field.Comment.Text);
                w.WriteLines(itemTags.ToBriefComment());
                w.WriteLine($"public {CSharpUtils.TypeTransform(field.Type)} {CSharpUtils.CSharpKeywordTransform(field.Name)};");
                if (i != @class.Fields.Count - 1) w.WriteLine();
            }
        }

        return new GeneratedUnit(@class.Name, group, headerFile, w.Lines);
    }
}
