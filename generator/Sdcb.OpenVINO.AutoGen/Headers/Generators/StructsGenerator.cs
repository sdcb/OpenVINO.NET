using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

public class StructsGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        Dictionary<Class, TypedefNameDecl> typeMappings = info.Units
            .SelectMany(x => x.Typedefs)
            .Where(x => x.Type is TagType { Declaration: Class })
            .GroupBy(x => (Class)((TagType)x.Type).Declaration)
            .ToDictionary(k => k.Key, v => v.First());

        Class[] structs = info.Units
            .SelectMany(x => x.Classes)
            .Where(x => !string.IsNullOrWhiteSpace(x.Name) || typeMappings.ContainsKey(x))
            .ToArray();
        Console.WriteLine($"Detected structs: {structs.Length}");
        return new(structs.Select(x => TransformOne(x, typeMappings)));
    }

    private static GeneratedUnit TransformOne(Class @class, Dictionary<Class, TypedefNameDecl> typeMappings)
    {
        TypedefNameDecl? typedef = typeMappings.GetValueOrDefault(@class);
        string name = string.IsNullOrWhiteSpace(@class.Name) ? typedef!.Name : @class.Name;

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
                w.WriteLine($"public {KnownTypeFix(name, field.Name, CSharpUtils.TypeTransform(field.Type))} {CSharpUtils.CSharpKeywordTransform(field.Name)};");
                if (i != @class.Fields.Count - 1) w.WriteLine();
            }
        }

        return new GeneratedUnit(name, group, @class.LineNumberStart, @class.LineNumberEnd, headerFile, w.Lines);
    }

    private static string KnownTypeFix(string structName, string fieldName, string type) => (structName, fieldName) switch
    {
        ("ov_dimension", "min" or "max") => "long",
        ("ov_profiling_info_t", "real_time" or "cpu_time") => "long",
        ("ov_shape_t", "rank") => "long",
        ("ov_shape_t", "dims") => "long*",
        _ => type,
    };
}
