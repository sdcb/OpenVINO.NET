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
        StringBuilder sb = new();
        using StringWriter sw = new(sb);
        using IndentedTextWriter w = new(sw, "    ");
        w.WriteLines(@class.Comment.ToBriefCode());
        w.WriteLine($"[StructLayout(LayoutKind.Sequential), CSourceInfo(\"{((TranslationUnit)@class.OriginalNamespace).FileName}\", {@class.LineNumberStart}, {@class.LineNumberEnd})]");

        string unsafeBlock = @class.Fields.Any(x => CSharpUtils.TypeTransform(x.Type).Contains('*')) ? "unsafe " : "";
        w.WriteLine($"public {unsafeBlock}struct {@class.Name}");
        w.BeginIdent(() =>
        {
            for (int i = 0; i < @class.Fields.Count; i++)
            {
                Field field = @class.Fields[i];
                w.WriteLines(field.Comment.ToSummaryCode());
                w.WriteLine($"public {CSharpUtils.TypeTransform(field.Type)} {CSharpUtils.CSharpKeywordTransform(field.Name)};");
                if (i != @class.Fields.Count - 1) w.WriteLine();
            }
        });

        return new GeneratedUnit(@class.Name, sb.ToString());
    }
}
