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

    public string Text => string.Join(Environment.NewLine + Environment.NewLine, this.Select(x => x.Text));
}

public record GeneratedUnit(string Name, string Text);
