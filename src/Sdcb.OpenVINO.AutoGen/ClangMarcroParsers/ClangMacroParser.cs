using FParsec.CSharp;
using Microsoft.FSharp.Core;
using Sdcb.OpenVINO.AutoGen.ClangMarcroParsers.Units;
using static FParsec.CharParsers;
using static FParsec.CSharp.CharParsersCS; // pre-defined parsers
using static FParsec.CSharp.PrimitivesCS; // combinator functions

namespace Sdcb.OpenVINO.AutoGen.ClangMarcroParsers
{
    public class ClangMacroParser
    {
        public static Func<string, IExpression> MakeParser()
        {
            HashSet<string> typeLiterals = "int64_t,UINT64_C".Split(',').OrderByDescending(x => x.Length).ToHashSet();
            FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<string>> notReserved(string id) => typeLiterals.Contains(id) ? Zero<string>() : Return(id);
            var identifier = Purify(Many1Chars(Choice(Letter, CharP('_')), Choice(Letter, CharP('_'), Digit))).AndTry(notReserved).And(WS).Lbl("identifier");
            var typeSyntax = Choice(typeLiterals.Select(t => StringP(t)).ToArray()).And(WS).Label("typeSyntax");

            NumberLiteralOptions numberLiteralOptions =
                (NumberLiteralOptions.AllowSuffix | NumberLiteralOptions.AllowHexadecimal | NumberLiteralOptions.DefaultFloat)
                & ~(NumberLiteralOptions.AllowMinusSign | NumberLiteralOptions.AllowPlusSign);

            var parser = new OPPBuilder<Unit, IExpression, Unit>()
                .WithOperators(ops => ops
                    .AddInfix(">", 10, WS, (x, y) => IExpression.MakeBinary(x, ">", y))
                    .AddInfix("<", 10, WS, (x, y) => IExpression.MakeBinary(x, "<", y))
                    .AddInfix("<<", 20, WS, (x, y) => IExpression.MakeBinary(x, "<<", y))
                    .AddInfix(">>", 20, WS, (x, y) => IExpression.MakeBinary(x, ">>", y))
                    .AddInfix("|", 30, WS, (x, y) => IExpression.MakeBinary(x, "|", y))
                    .AddInfix("+", 30, WS, (x, y) => IExpression.MakeBinary(x, "+", y))
                    .AddInfix("-", 30, WS, (x, y) => IExpression.MakeBinary(x, "-", y))
                    .AddInfix("*", 40, WS, (x, y) => IExpression.MakeBinary(x, "*", y))
                    .AddInfix("/", 40, WS, (x, y) => IExpression.MakeBinary(x, "/", y))
                    .AddPrefix("-", 40, WS, IExpression.MakeNegative)
                    )
                .WithImplicitOperator(50, IExpression.FromImplicitBinary)
                .WithTerms((FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<IExpression>> term) =>
                {
                    var parenthese1 = Between(CharP('(').And(WS), term, CharP(')').And(WS));
                    var parentheseN = Between(CharP('(').And(WS), Many(term, CharP(',').And(WS)), CharP(')').And(WS));

                    return Choice(
                        Try(Between(CharP('(').And(WS), typeSyntax, CharP(')'))).And(term).And(WS).Map((id, val) => IExpression.MakeTypeCast(id, val)),
                        Try(identifier.And(parentheseN)).Map((id, val) => IExpression.MakeCall(id, val.ToArray())),
                        Between('\'', AnyChar, '\'').And(WS).Map(x => IExpression.MakeChar(x)),
                        Between('"', ManyChars(NoneOf("\"")), '"').And(WS).Map(x => IExpression.MakeString(x)),
                        NumberLiteral(numberLiteralOptions, "Number").And(WS).Map(x => IExpression.MakeNumber(x)),
                        parenthese1.Map(x => IExpression.MakeParenthese(x)),
                        typeSyntax.And(parenthese1).Map((id, val) => IExpression.MakeTypeCast(id, val)),
                        identifier.Map(x => IExpression.MakeIdentifier(x))
                    ).Label("expression");
                })
                .Build()
                .ExpressionParser;

            return (string str) => parser.ParseString(Preprocess(str)) switch
            {
                { Status: FParsec.ReplyStatus.Ok } x => x.Result, 
                var x => throw new NotSupportedException(string.Join("\n", FParsec.ErrorMessageList.ToHashSet(x.Error).Select(x => x.Type.ToString())))
            };

            static string Preprocess(string raw) => raw.Replace("\\\n", " ");
        }
    }
}
