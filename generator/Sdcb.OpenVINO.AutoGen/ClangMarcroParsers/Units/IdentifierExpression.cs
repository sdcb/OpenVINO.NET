namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public record IdentifierExpression(string Name) : IExpression
    {
        public string Serialize() => Name;
    }
}
