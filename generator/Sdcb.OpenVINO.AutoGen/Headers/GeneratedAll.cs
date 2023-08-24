namespace Sdcb.OpenVINO.AutoGen.Headers;

public record GeneratedAll(GeneratedUnits Functions, GeneratedUnits Enums, GeneratedUnits Structs)
{
    public static GeneratedAll Generate(ParsedInfo info)
    {
        GeneratedUnits functions = FunctionGenerator.Run(info);
        return new GeneratedAll(functions, new(), new());
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
