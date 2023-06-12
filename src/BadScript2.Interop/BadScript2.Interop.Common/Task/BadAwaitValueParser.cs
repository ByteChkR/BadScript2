using BadScript2.Common;
using BadScript2.Parser;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Operators;
using BadScript2.Reader;

namespace BadScript2.Interop.Common.Task;

/// <summary>
///     Implements the parse for the 'await' Expression
/// </summary>
public class BadAwaitValueParser : BadValueParser
{
	public override bool IsValue(BadSourceParser parser)
	{
		return parser.Reader.Is("await");
	}

	public override BadExpression ParseValue(BadSourceParser parser)
	{
		BadSourcePosition pos = parser.Reader.Eat("await");
		parser.Reader.SkipNonToken();
		BadExpression expr = parser.ParseExpression();

		return new BadAwaitExpression(expr, pos.Combine(expr.Position));
	}
}
