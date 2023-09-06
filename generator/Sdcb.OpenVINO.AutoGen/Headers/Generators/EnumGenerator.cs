using CppSharp.AST;
using Sdcb.OpenVINO.AutoGen.Writers;
using System;
using System.CodeDom.Compiler;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

public class EnumGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Enumeration[] rawEnums = info.Units
                    .SelectMany(x => x.Enums)
                    .ToArray();
        Console.WriteLine($"Detected enums: {rawEnums.Length}");
        GeneratedUnits enums = new(rawEnums.Select(TransformOne));
        return enums;
    }

    private static GeneratedUnit TransformOne(Enumeration @enum)
    {
        string headerFile = ((TranslationUnit)@enum.OriginalNamespace).FileName;

        IndentedLinesWriter w = new();

        w.WriteLines(@enum.Comment.ToBriefCode());

        VerbatimLineComment? groupBlock = @enum.Comment?.FullComment.Blocks
            .OfType<VerbatimLineComment>()
            .Where(x => x.CommandKind == CommentCommandKind.A && x.Text.Trim() != @enum.Name)
            .SingleOrDefault();
        string? group = groupBlock?.Text.Trim();

        w.WriteLine($"[CSourceInfo(\"{headerFile}\", {@enum.LineNumberStart}, {@enum.LineNumberEnd}, \"{group}\")]");
        w.WriteLine($"public enum {@enum.Name}");
        using (w.BeginIdent())
        {
            for (int i = 0; i < @enum.Items.Count; i++)
            {
                Enumeration.Item item = @enum.Items[i];
                w.WriteLines(item.Comment.ToSummaryCode());
                w.WriteLine($"{item.Name} = {ConvertValue(item.Value, @enum.BuiltinType.Type)},");
                if (i != @enum.Items.Count - 1) w.WriteLine();
            }
        };

        return new GeneratedUnit(@enum.Name, group, headerFile, w.Lines);
    }

    private static object ConvertValue(ulong value, PrimitiveType primitiveType)
    {
        return primitiveType switch
        {
            PrimitiveType.Int => value > int.MaxValue ? (int)value : value,
            PrimitiveType.UInt => value,
            PrimitiveType.Long => value > long.MaxValue ? (long)value : value,
            PrimitiveType.ULong => value,
            _ => throw new NotSupportedException()
        };
    }
}
