using BadScript2.Common;
using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

public class BadTypeOfExpressionParser : BadValueParser
{
	public int Precedence => 3;

	public override bool IsValue(BadSourceParser parser)
	{
		return parser.Reader.Is("typeof");
	}

	public override BadExpression ParseValue(BadSourceParser parser)
	{
		BadSourcePosition pos = parser.Reader.Eat("typeof");
		BadExpression expr = parser.ParseExpression(null, Precedence);

		return new BadTypeOfExpression(expr, pos.Combine(expr.Position));
	}
}
