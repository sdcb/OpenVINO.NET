using CppSharp.AST;

namespace Sdcb.OpenVINO.AutoGen.Headers;

internal class FunctionGenerator
{
    public static GeneratedUnits Run(ParsedInfo info)
    {
        Function[] rawFuncs = info.Units
                    .SelectMany(x => x.Functions)
                    .Where(x => info.Symbols.Contains(x.Name))
                    .ToArray();
        Console.WriteLine($"Detected functions: {info.Units.SelectMany(x => x.Functions).Count()}, matched: {rawFuncs.Length} in symbols");
        GeneratedUnits functions = new(rawFuncs.Select(TransformOneFunction));
        return functions;
    }

    static GeneratedUnit TransformOneFunction(Function function)
    {
        string paramsText = function.Comment.FullComment.Blocks
            .OfType<ParamCommandComment>()
            .Select(x => $"/// <param name=\"{x.Arguments[0].Text}\">{string.Join(" ", x.ParagraphComment.Content.OfType<TextComment>().Select(x => x.Text)).Trim()}</param>")
            .Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");

        paramsText = paramsText == null ? "" : Environment.NewLine + paramsText;
        return new GeneratedUnit(function.Name, $"""
            /// <summary>{function.Comment.BriefText}</summary>{paramsText}
            {CSharpUtils.TypeTransform(function.ReturnType.Type)} {function.Name}({string.Join(", ", function.Parameters.Select(TransformOneParameter))});
            """);
    }

    static string TransformOneParameter(Parameter p)
    {
        return $"{CSharpUtils.TypeTransform(p.QualifiedType.Type)} {p.Name}";
    }
}
