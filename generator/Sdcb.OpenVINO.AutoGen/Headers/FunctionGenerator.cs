using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers;

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
        string paramsText = func.Comment.FullComment.Blocks
            .OfType<ParamCommandComment>()
            .Select(x => $"/// <param name=\"{x.Arguments[0].Text}\">{string.Join(" ", x.ParagraphComment.Content.OfType<TextComment>().Select(x => x.Text)).Trim()}</param>")
            .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");

        paramsText = paramsText == null ? "" : Environment.NewLine + paramsText;
        return new GeneratedUnit(func.Name, $"""
            /// <summary>{func.Comment.BriefText}</summary>{paramsText}
            [DllImport(Dll), CSourceInfo("{((TranslationUnit)func.OriginalNamespace).FileName}", {func.LineNumberStart}, {func.LineNumberEnd})]
            public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", func.Parameters.Select(TransformOneParameter))});
            """);
    }

    static string TransformOneParameter(Parameter p)
    {
        return $"{CSharpUtils.TypeTransform(p.QualifiedType.Type)} {CSharpUtils.CSharpKeywordTransform(p.Name)}";
    }
}
