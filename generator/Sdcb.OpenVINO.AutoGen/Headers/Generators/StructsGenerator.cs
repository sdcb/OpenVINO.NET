using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

public class StructsGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Dictionary<string, TypedefNameDecl> typeMappings = info.Units
            .SelectMany(x => x.Typedefs)
            .Where(x => x.Type is TagType)
            .ToDictionary(k => ((TagType)k.Type).Declaration.Name, v => v);

        Class[] structs = info.Units
            .SelectMany(x => x.Classes)
            .ToArray();
        Console.WriteLine($"Detected structs: {structs.Length}");
        return new(structs.Select(x => TransformOne(x, typeMappings)));
    }

    private static GeneratedUnit TransformOne(Class @class, Dictionary<string, TypedefNameDecl> typeMappings)
    {
        TypedefNameDecl? typedef = typeMappings.GetValueOrDefault(@class.Name);
        string name = @class.Name;

        IndentedLinesWriter w = new();
        DoxygenTags tags = DoxygenTags.Parse(typedef?.Comment?.Text ?? @class.Comment?.Text);
        w.WriteLines(tags.ToBriefComment());

        string? group = tags.Group;
        string headerFile = ((TranslationUnit)@class.OriginalNamespace).FileName;

        w.WriteLine($"[StructLayout(LayoutKind.Sequential), CSourceInfo(\"{headerFile}\", {@class.LineNumberStart}, {@class.LineNumberEnd}, \"{group}\")]");

        string unsafeBlock = @class.Fields.Any(x => CSharpUtils.TypeTransform(x.Type).Contains('*')) ? "unsafe " : "";
        w.WriteLine($"public {unsafeBlock}struct {name}");
        using (w.BeginIdent())
        {
            for (int i = 0; i < @class.Fields.Count; i++)
            {
                Field field = @class.Fields[i];
                DoxygenTags itemTags = DoxygenTags.Parse(field.Comment?.Text);
                w.WriteLines(itemTags.ToBriefComment());
                w.WriteLine($"public {CSharpUtils.TypeTransform(field.Type)} {CSharpUtils.CSharpKeywordTransform(field.Name)};");
                if (i != @class.Fields.Count - 1) w.WriteLine();
            }
        }

        return new GeneratedUnit(name, group, @class.LineNumberStart, @class.LineNumberEnd, headerFile, w.Lines);
    }
}
