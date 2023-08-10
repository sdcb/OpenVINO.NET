<Query Kind="Statements">
  <NuGetReference>FParsec.CSharp</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>FParsec.CSharp; // extension functions (combinators &amp; helpers)</Namespace>
  <Namespace>Microsoft.FSharp.Core</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>static FParsec.CharParsers</Namespace>
  <Namespace>static FParsec.CSharp.CharParsersCS; // pre-defined parsers</Namespace>
  <Namespace>static FParsec.CSharp.PrimitivesCS; // combinator functions</Namespace>
</Query>

var parser = MakeParser();
var data = JsonConvert.DeserializeObject<MacroResult[]>(File.ReadAllText(@"C:\Users\ZhouJie\Documents\WeChat Files\wxid_1hwm3oil9a7512\FileStorage\File\2022-08\macro.json"))
	.Where(x => x.Valid)
	//.Take(50)
	.Select(x => new
	{
		Name = x.Name,
		TheirParsed = x.Expression,
		MyParsed = Parse(parser, x.Raw),
		Raw = x.Raw,
	})
	.Dump()
	.ToArray();

Console.WriteLine($"Total={data.Length}, Parsed={data.Count(x => x.MyParsed is string)}");
//Parse(parser, "(AV_CH_FRONT_LEFT|AV_CH_FRONT_RIGHT)").Dump();



FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<Expression>> MakeParser()
{
	HashSet<string> typeLiterals = "int64_t,UINT64_C".Split(',').OrderByDescending(x => x.Length).ToHashSet();
	FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<string>> notReserved(string id) => typeLiterals.Contains(id) ? Zero<string>() : Return(id);
	var identifier = Purify(Many1Chars(Choice(Letter, CharP('_')), Choice(Letter, CharP('_'), Digit))).AndTry(notReserved).And(WS).Lbl("identifier");
	var typeSyntax = Choice(typeLiterals.Select(t => StringP(t)).ToArray()).And(WS).Label("typeSyntax");
	
	NumberLiteralOptions numberLiteralOptions = 
		(NumberLiteralOptions.AllowSuffix | NumberLiteralOptions.AllowHexadecimal | NumberLiteralOptions.DefaultFloat) 
		& ~(NumberLiteralOptions.AllowMinusSign | NumberLiteralOptions.AllowPlusSign);

	return new OPPBuilder<Unit, Expression, Unit>()
		.WithOperators(ops => ops
			.AddInfix(">", 10, WS, (x, y) => Expression.MakeBinary(x, new Operator(">"), y))
			.AddInfix("<", 10, WS, (x, y) => Expression.MakeBinary(x, new Operator("<"), y))
			.AddInfix("<<", 20, WS, (x, y) => Expression.MakeBinary(x, new Operator("<<"), y))
			.AddInfix(">>", 20, WS, (x, y) => Expression.MakeBinary(x, new Operator(">>"), y))
			.AddInfix("|", 30, WS, (x, y) => Expression.MakeBinary(x, new Operator("|"), y))
			.AddInfix("+", 30, WS, (x, y) => Expression.MakeBinary(x, new Operator("+"), y))
			.AddInfix("-", 30, WS, (x, y) => Expression.MakeBinary(x, new Operator("-"), y))
			.AddInfix("*", 40, WS, (x, y) => Expression.MakeBinary(x, new Operator("*"), y))
			.AddInfix("/", 40, WS, (x, y) => Expression.MakeBinary(x, new Operator("/"), y))
			.AddPrefix("-", 40, WS, (x) => Expression.MakeNegative(x))
			)
		.WithImplicitOperator(50, (e1, e2) => ImplicitExpression.FromBinary(e1, e2))
		.WithTerms((FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<Expression>> term) =>
		{
			var parenthese1 = Between(CharP('(').And(WS), term, CharP(')').And(WS));
			var parentheseN = Between(CharP('(').And(WS), Many(term, CharP(',').And(WS)), CharP(')').And(WS));
			
			return PrimitivesCS.Choice(
				Try(Between(CharP('(').And(WS), typeSyntax, CharP(')'))).And(term).And(WS).Map((id, val) => Expression.MakeTypeConvert(id, val)), 
				Try(identifier.And(parentheseN)).Map((id, val) => Expression.MakeFunctionCall(id, val.ToArray())), 
				Between('\'', AnyChar, '\'').And(WS).Map(x => Expression.MakeCharLiteral(x)),
				Between('"', ManyChars(NoneOf("\"")), '"').And(WS).Map(x => Expression.MakeStringLiteral(x)),
				NumberLiteral(numberLiteralOptions, "Number").And(WS).Map(x => Expression.MakeNumberLiteral(x)),
				parenthese1.Map(x => Expression.MakeParenthese(x)),
				typeSyntax.And(parenthese1).Map((id, val) => Expression.MakeTypeConvert(id, val)), 
				identifier.Map(x => Expression.MakeIdentifier(x)) 
			).Label("expression");
		})
		.Build()
		.ExpressionParser;
}

object Parse(FSharpFunc<FParsec.CharStream<Unit>, FParsec.Reply<Expression>> parser, string raw)
{
	string inline = raw.Replace("\\\n", "");
	return parser.ParseString(inline) switch
	{
		{ Status: FParsec.ReplyStatus.Ok } x => x.Result.Serialize(), 
		//var x => throw new Exception(string.Join(",", FParsec.ErrorMessageList.ToHashSet(x.Error).Select(x => x.Type.ToString())) + $"\nraw: {inline}"), 
		var x => new 
		{ 
			Error = FParsec.ErrorMessageList.ToHashSet(x.Error).Select(x => x.Type), 
			Raw = inline
		}
		//_ => null, 
	};
}

static class ImplicitExpression
{
	public static Expression FromBinary(Expression left, Expression right)
	{
		return (left, right) switch
		{
			(StringLiteralExpression str, Expression exp) => new StringConcatExpression(str, exp), 
			//_ => left,
			_ => throw new NotSupportedException($"left {left.GetType().Name}({left.Serialize()}), right: {right.GetType().Name}({right.Serialize()}) is not supported"), 
		};
	}
}

abstract record Token
{
	public abstract string Serialize();
}
abstract record Expression : Token
{
	public static Expression MakeNumberLiteral(NumberLiteral number) => new NumberLiteralExpression(number);
	public static Expression MakeStringLiteral(string text) => new StringLiteralExpression(text);
	public static Expression MakeStringConcat(StringLiteralExpression str, Expression exp) => new StringConcatExpression(str, exp);
	public static Expression MakeCharLiteral(char c) => new CharLiteralExpression(c);
	public static Expression MakeNegative(Expression e) => new NegativeExpression(e);
	public static Expression MakeIdentifier(string identifier) => new IdentifierExpression(identifier);
	public static Expression MakeBinary(Expression left, Operator op, Expression right) => new BinaryExpression(left, op, right);
	public static Expression MakeParenthese(Expression val) => new ParentheseExpression(val);
	public static Expression MakeFunctionCall(string functionName, Expression[] args) => new FunctionCallExpression(functionName, args);
	public static Expression MakeTypeConvert(string destType, Expression exp) => new TypeConvertExpression(destType, exp);
}

record NumberLiteralExpression(NumberLiteral number) : Expression
{
	public override string Serialize() => number.String;
}

record StringLiteralExpression(string c) : Expression
{
	public override string Serialize() =>$"\"{c}\"";
}

record StringConcatExpression(StringLiteralExpression str, Expression exp) : Expression
{
	public override string Serialize() => $"{str.Serialize()} + {exp.Serialize()}";
}

record CharLiteralExpression(char c) : Expression
{
	public override string Serialize() => $"'{c}'";
}

record Operator(string op) : Token
{
	public override string Serialize() => op;
}

record NegativeExpression(Expression val) : Expression
{
	public override string Serialize() => $"-{val.Serialize()}";
}

record IdentifierExpression(string identifier) : Expression
{	
	public override string Serialize() => identifier;
}

record BinaryExpression(Expression left, Operator op, Expression right) : Expression
{
	public override string Serialize() => $"{left.Serialize()} {op.Serialize()} {right.Serialize()}";
}

record ParentheseExpression(Expression content) : Expression
{
	public override string Serialize() => $"({content.Serialize()})";
}

record FunctionCallExpression(string identifier, Expression[] arguments) : Expression
{
	public override string Serialize() => $"{identifier}({string.Join(", ", arguments.Select(x => x.Serialize()))})";
}

record TypeConvertExpression(string destType, Expression exp) : Expression
{
	public override string Serialize() => $"({destType}){exp.Serialize()}";
}

record MacroResult
{
	public string Name { get; init; }
	public bool Valid { get; init; }
	public string Expression { get; init; }
	public string Raw { get; init; }
	public string Exception { get; init; }
}