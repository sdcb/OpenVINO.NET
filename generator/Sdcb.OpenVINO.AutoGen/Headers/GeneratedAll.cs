using Sdcb.OpenVINO.AutoGen.Headers.Generators;

namespace Sdcb.OpenVINO.AutoGen.Headers;

public record GeneratedAll(GeneratedUnits Functions, GeneratedUnits Enums, GeneratedUnits Structs)
{
    public static GeneratedAll Generate(ParsedInfo info)
    {
        GeneratedUnits functions = FunctionGenerator.Generate(info);
        GeneratedUnits enums = EnumGenerator.Generate(info);
        GeneratedUnits structs = StructsGenerator.Generate(info);
        return new GeneratedAll(functions, enums, structs);
    }
}

public class GeneratedUnits : List<GeneratedUnit>
{
    public GeneratedUnits(IEnumerable<GeneratedUnit> collection) : base(collection)
    {
    }

    public GeneratedUnits() : base()
    {
    }

    public IEnumerable<string> Lines => this
        .OrderBy(x => x.HeaderFile)
        .ThenBy(x => x.LineNumberStart)
        //.ThenBy(x => x.Name)
        .Aggregate(Enumerable.Empty<string>(), (a, b) => a.Concat(new[] { Environment.NewLine }).Concat(b.Lines));
}

public record GeneratedUnit(string Name, string? Group, int LineNumberStart, int LineNumberEnd, string HeaderFile, IEnumerable<string> Lines);
