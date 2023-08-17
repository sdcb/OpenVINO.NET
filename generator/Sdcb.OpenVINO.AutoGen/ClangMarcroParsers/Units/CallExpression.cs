namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public record CallExpression(string FunctionName, params IExpression[] Arguments) : IExpression
    {
        public string Serialize() => $"{FunctionName}({string.Join(", ", Arguments.Select(x => x.Serialize()))})";
    }
}
