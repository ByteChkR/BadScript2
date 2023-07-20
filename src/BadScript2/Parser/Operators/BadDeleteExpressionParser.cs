using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Reader;

namespace BadScript2.Parser.Operators;

public class BadDeleteExpressionParser : BadValueParser
{
	public int Precedence => 3;

	public override bool IsValue(BadSourceParser parser)
	{
		return parser.Reader.IsKey("delete");
	}

	public override BadExpression ParseValue(BadSourceParser parser)
	{
		BadSourcePosition pos = parser.Reader.Eat("delete");
		BadExpression expr = parser.ParseExpression(null, Precedence);

		return new BadDeleteExpression(expr, pos.Combine(expr.Position));
	}
}
