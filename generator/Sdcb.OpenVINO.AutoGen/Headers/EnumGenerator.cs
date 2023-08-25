using CppSharp.AST;
using Sdcb.OpenVINO.AutoGen.Writers;
using System.CodeDom.Compiler;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public class EnumGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Enumeration[] rawEnums = info.Units
                    .SelectMany(x => x.Enums)
                    .OrderBy(x => ((TranslationUnit)x.OriginalNamespace).FileName)
                    .ToArray();
        Console.WriteLine($"Detected enums: {info.Units.SelectMany(x => x.Enums).Count()}");
        GeneratedUnits enums = new(rawEnums.Select(TransformOneEnum));
        return enums;
    }

    private static GeneratedUnit TransformOneEnum(Enumeration @enum)
    {
        var items = @enum.Items.Select(x => new
        {
            x.Name,
            Comment = x.Comment.FullComment.Blocks
                .OfType<ParagraphComment>()
                .Select(x => x.Content.OfType<TextComment>().Select(x => x.Text).Aggregate((x, y) => $"{x}{Environment.NewLine}{y}"))
                .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}").Trim(),
            Value = ConvertValue(x.Value, @enum.BuiltinType.Type),
        }).ToArray();

        StringBuilder sb = new();
        using StringWriter sw = new(sb);
        using IndentedTextWriter w = new(sw, "    ");
        if (@enum.Comment != null)
        {
            w.WriteLine($"/// <summary>{@enum.Comment.BriefText}</summary>");
        }
        else
        {
            w.WriteLine($"/// <summary>enum: {@enum.Name}</summary>");
        }
        w.WriteLine($"[CSourceInfo(\"{((TranslationUnit)@enum.OriginalNamespace).FileName}\", {@enum.LineNumberStart}, {@enum.LineNumberEnd})]");
        w.WriteLine($"public enum {@enum.Name}");
        w.BeginIdent(() =>
        {
            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];
                w.WriteLine($"/// <summary>{item.Comment}</summary>");
                w.WriteLine($"{item.Name} = {item.Value},");
                if (i != items.Length - 1) w.WriteLine();
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
