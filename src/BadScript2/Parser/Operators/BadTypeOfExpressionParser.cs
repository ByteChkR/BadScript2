using BadScript2.Common;
using BadScript2.Parser.Expressions;
using BadScript2.Reader;

namespace BadScript2.Parser.Operators;
/// <summary>
/// Implements the Type Of Expression Parser
/// </summary>
public class BadTypeOfExpressionParser : BadValueParser
{

	public override bool IsValue(BadSourceParser parser)
	{
		return parser.Reader.IsKey("typeof");
	}

	public override BadExpression ParseValue(BadSourceParser parser)
	{
		BadSourcePosition pos = parser.Reader.Eat("typeof");
		BadExpression expr = parser.ParseExpression(null, 3);

		return new BadTypeOfExpression(expr, pos.Combine(expr.Position));
	}
}
