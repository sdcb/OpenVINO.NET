using CppSharp.AST;

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
            realParams.Add(RealFuncParam.Variadic);
        }

        DoxygenTags tags = DoxygenTags.Parse(func.Comment.Text);
        w.WriteLines(tags.ToBriefComment());
        w.WriteLines(tags.ToParamsComment(realParams));
        w.WriteLines(tags.ToReturnsComment());

        string? group = tags.Group;
        string headerFile = ((TranslationUnit)func.OriginalNamespace).FileName;

        w.WriteLine($"[DllImport(Dll, CallingConvention = CallingConvention.Cdecl), CSourceInfo(\"{headerFile}\", {func.LineNumberStart}, {func.LineNumberEnd}, \"{group}\")]");
        w.WriteLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", realParams.Where(x => !x.IsVariadic))});");

        if (func.IsVariadic)
        {
            if (func.Name == "ov_preprocess_input_tensor_info_set_color_format_with_subname")
            {
                for (int i = 0; i < 10; ++i)
                {
                    w.Write($"[DllImport(Dll, EntryPoint = nameof({func.Name}), CallingConvention = CallingConvention.Cdecl)] ");
                    string variadics = string.Join(", ", Enumerable.Range(0, i + 1).Select(x => $"IntPtr varg{x + 1}"));
                    w.WriteLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", realParams.Where(x => !x.IsVariadic))}, {variadics});");
                }
            }
            else if (func.Name == "ov_compiled_model_set_property")
            {
                w.Write($"[DllImport(Dll, EntryPoint = nameof({func.Name}), CallingConvention = CallingConvention.Cdecl)] ");
                w.WriteLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", realParams.Where(x => !x.IsVariadic))}, IntPtr key, IntPtr value);");
            }
            else
            {
                for (int i = 0; i < 10; ++i)
                {
                    w.Write($"[DllImport(Dll, EntryPoint = nameof({func.Name}), CallingConvention = CallingConvention.Cdecl)] ");
                    string variadics = string.Join(", ", Enumerable.Range(0, i + 1).Select(x => $"IntPtr varg{x * 2 + 1}, IntPtr varg{x * 2 + 2}"));
                    w.WriteLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", realParams.Where(x => !x.IsVariadic))}, {variadics});");
                }
            }
        }
        return new GeneratedUnit(func.Name, group, func.LineNumberStart, func.LineNumberEnd, headerFile, w.Lines);
    }
}

public record RealFuncParam(string Type, string NameUnescaped)
{
    public string Name => CSharpUtils.CSharpKeywordTransform(NameUnescaped);

    public static explicit operator RealFuncParam(Parameter p) => new(CSharpUtils.TypeTransform(p.QualifiedType.Type), p.Name);

    public static RealFuncParam Variadic { get; } = new("__arglist", "");

    public bool IsVariadic => Type == Variadic.Type;

    public override string ToString() => IsVariadic ? Variadic.Type : $"{Type} {Name}";
}