using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

public class BadDeleteExpressionParser : BadValueParser
{
	public int Precedence => 3;
	public override bool IsValue(BadSourceParser parser)
	{
		return parser.Reader.Is("delete");
	}

	public override BadExpression ParseValue(BadSourceParser parser)
	{
		var pos = parser.Reader.Eat("delete");
		var expr = parser.ParseExpression(null, Precedence);

		return new BadDeleteExpression(expr, pos.Combine(expr.Position));
	}
}