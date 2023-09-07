using CppSharp.AST;
using System.Security;
using System.Text;

namespace Sdcb.OpenVINO.AutoGen.Headers.Generators;

internal class FunctionGenerator
{
    public static GeneratedUnits Generate(ParsedInfo info)
    {
        var structs = info.Units
            .SelectMany(x => x.Classes)
            .ToArray();

        Function[] rawFuncs = info.Units
                    .SelectMany(x => x.Functions)
                    .Where(x => info.Symbols.Contains(x.Name))
                    .OrderBy(x => ((TranslationUnit)x.OriginalNamespace).FileName)
                    .ToArray();
        Console.WriteLine($"Detected functions: {info.Units.SelectMany(x => x.Functions).Count()}, matched: {rawFuncs.Length} in symbols");
        GeneratedUnits functions = new(rawFuncs.Select(TransformOneFunction));
        return functions;
    }

    static GeneratedUnit TransformOneFunction(Function func)
    {
        IndentedLinesWriter w = new();

        List<RealFuncParam> realParams = func.Parameters
            .Select(x => (RealFuncParam)x)
            .ToList();
        if (func.IsVariadic)
        {
            realParams.Add(new("IntPtr", "variadic"));
        }

        DoxygenTags tags = DoxygenTags.Parse(func.Comment.Text);
        w.WriteLines(tags.ToBriefComment());
        w.WriteLines(tags.ToParamsComment(realParams));
        w.WriteLines(tags.ToReturnsComment());

        string? group = tags.Group;
        string headerFile = ((TranslationUnit)func.OriginalNamespace).FileName;

        w.WriteLine($"[DllImport(Dll), CSourceInfo(\"{headerFile}\", {func.LineNumberStart}, {func.LineNumberEnd}, \"{group}\")]");
        w.WriteLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", realParams)});");
        return new GeneratedUnit(func.Name, group, headerFile, w.Lines);
    }
}

public record RealFuncParam(string Type, string NameUnescaped)
{
    public string Name => CSharpUtils.CSharpKeywordTransform(NameUnescaped);

    public static explicit operator RealFuncParam(Parameter p) => new(CSharpUtils.TypeTransform(p.QualifiedType.Type), p.Name);

    public override string ToString() => $"{Type} {Name}";
}