using CppSharp.AST;
using Sdcb.OpenVINO.AutoGen.Writers;
using System.CodeDom.Compiler;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

public class EnumGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Enumeration[] rawEnums = info.Units
                    .SelectMany(x => x.Enums)
                    .OrderBy(x => ((TranslationUnit)x.OriginalNamespace).FileName)
                    .ToArray();
        Console.WriteLine($"Detected enums: {rawEnums.Length}");
        GeneratedUnits enums = new(rawEnums.Select(TransformOne));
        return enums;
    }

    private static GeneratedUnit TransformOne(Enumeration @enum)
    {
        StringBuilder sb = new();
        using StringWriter sw = new(sb);
        using IndentedTextWriter w = new(sw, "    ");

        w.WriteLines(@enum.Comment.ToBriefCode());
        w.WriteLine($"[CSourceInfo(\"{((TranslationUnit)@enum.OriginalNamespace).FileName}\", {@enum.LineNumberStart}, {@enum.LineNumberEnd})]");
        w.WriteLine($"public enum {@enum.Name}");
        w.BeginIdent(() =>
        {
            for (int i = 0; i < @enum.Items.Count; i++)
            {
                Enumeration.Item item = @enum.Items[i];
                w.WriteLines(item.Comment.ToSummaryCode());
                w.WriteLine($"{item.Name} = {ConvertValue(item.Value, @enum.BuiltinType.Type)},");
                if (i != @enum.Items.Count - 1) w.WriteLine();
            }
        });

        return new GeneratedUnit(@enum.Name, sb.ToString());
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
