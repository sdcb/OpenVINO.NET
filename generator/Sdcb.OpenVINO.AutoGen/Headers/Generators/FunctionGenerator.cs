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
        StringBuilder body = new();
        if (func.Comment.BriefText != null)
        {
            body.AppendLine($"/// <summary>{XmlEscape(func.Comment.BriefText)}</summary>");
        }

        List<FunctionParameter> allParams = func.Parameters
            .Select(x => (FunctionParameter)x)
            .ToList();
        if (func.IsVariadic)
        {
            allParams.Add(new("IntPtr", "variadic"));
        }

        foreach ((ParamCommandComment x, int i) v in func.Comment.FullComment.Blocks
            .OfType<ParamCommandComment>()
            .Select((x, i) => (x, i)))
        {
            string raw = string.Concat(v.x.ParagraphComment.Content.OfType<TextComment>().Select(x => x.Text)).Trim();
            body.AppendLine($"/// <param name=\"{allParams[v.i].NameUnescaped}\">{XmlEscape(raw)}</param>");
        }

        BlockCommandComment? returnBlock = func.Comment.FullComment.Blocks
            .OfType<BlockCommandComment>()
            .SingleOrDefault(x => x.CommandKind == CommentCommandKind.Return);
        if (returnBlock != null)
        {
            string detail = string.Concat(returnBlock.ParagraphComment.Content.OfType<TextComment>().Select(x => x.Text)).Trim();
            body.AppendLine($"/// <returns>{XmlEscape(detail)}</returns>");
        }

        VerbatimLineComment? groupBlock = func.Comment.FullComment.Blocks
            .OfType<VerbatimLineComment>()
            .SingleOrDefault(x => x.CommandKind == CommentCommandKind.A);
        if (groupBlock != null)
        {
            body.AppendLine($"[DllImport(Dll), CSourceInfo(\"{((TranslationUnit)func.OriginalNamespace).FileName}\", {func.LineNumberStart}, {func.LineNumberEnd}, \"{groupBlock.Text.Trim()}\")]");
        }
        else
        {
            body.AppendLine($"[DllImport(Dll), CSourceInfo(\"{((TranslationUnit)func.OriginalNamespace).FileName}\", {func.LineNumberStart}, {func.LineNumberEnd})]");
        }
        body.AppendLine($"public static extern {CSharpUtils.TypeTransform(func.ReturnType.Type)} {func.Name}({string.Join(", ", allParams)});");
        return new GeneratedUnit(func.Name, body.ToString());
    }

    static string XmlEscape(string s) => SecurityElement.Escape(s);

    record FunctionParameter(string Type, string NameUnescaped)
    {
        public string Name => CSharpUtils.CSharpKeywordTransform(NameUnescaped);

        public static explicit operator FunctionParameter(Parameter p) => new(CSharpUtils.TypeTransform(p.QualifiedType.Type), p.Name);

        public override string ToString() => $"{Type} {Name}";
    }
}