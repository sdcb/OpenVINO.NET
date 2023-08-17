using static FParsec.CharParsers;

namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units
{
    public interface IExpression
    {
        public string Serialize();

        public static IExpression MakeNumber(NumberLiteral number) => new NumberExpression(number);
        public static IExpression MakeString(string text) => new StringExpression(text);
        public static IExpression MakeStringConcat(StringExpression str, IExpression exp) => new StringConcatExpression(str, exp);
        public static IExpression MakeChar(char c) => new CharExpression(c);
        public static IExpression MakeNegative(IExpression e) => new NegativeExpression(e);
        public static IExpression MakeIdentifier(string identifier) => new IdentifierExpression(identifier);
        public static IExpression MakeBinary(IExpression left, string op, IExpression right) => new BinaryExpression(left, op, right);
        public static IExpression MakeParenthese(IExpression val) => new ParentheseExpression(val);
        public static IExpression MakeCall(string functionName, IExpression[] args) => new CallExpression(functionName, args);
        public static IExpression MakeTypeCast(string destType, IExpression exp) => new TypeCastExpression(destType, exp);
        public static IExpression MakeTenary(IExpression conditional, IExpression trueResult, IExpression falseResult) => new TernaryExpression(conditional, trueResult, falseResult);

        public static IExpression FromImplicitBinary(IExpression left, IExpression right)
        {
            return (left, right) switch
            {
                (StringExpression str, IExpression exp) => new StringConcatExpression(str, exp),
                //_ => left,
                _ => throw new NotSupportedException($"left {left.GetType().Name}({left.Serialize()}), right: {right.GetType().Name}({right.Serialize()}) is not supported"),
            };
        }
    }
}
