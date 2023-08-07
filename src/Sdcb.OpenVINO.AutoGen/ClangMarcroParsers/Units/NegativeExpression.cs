namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public record NegativeExpression(IExpression Val) : IExpression
    {
        public string Serialize() => $"-{Val.Serialize()}";
    }
}
