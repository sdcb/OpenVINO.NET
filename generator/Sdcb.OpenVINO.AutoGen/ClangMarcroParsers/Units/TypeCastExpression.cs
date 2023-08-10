namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public record TypeCastExpression(string DestType, IExpression Exp) : IExpression
    {
        public string Serialize() => Exp switch
        {
            NumberExpression or CharExpression or StringExpression or IdentifierExpression => $"({DestType}){Exp.Serialize()}", 
            _ => $"({DestType})({Exp.Serialize()})"
        };
    }
}
