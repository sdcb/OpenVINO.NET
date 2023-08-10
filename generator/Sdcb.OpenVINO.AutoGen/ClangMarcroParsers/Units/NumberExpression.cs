using static FParsec.CharParsers;

namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public record NumberExpression(NumberLiteral Number) : IExpression
    {
        public string Serialize() => Number.String;
    }
}
