using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the '...' operator.
/// </summary>
public class BadUnaryUnpackOperator : BadUnaryPrefixOperator
{
	/// <summary>
	///     Creates a new '...' operator
	/// </summary>
	public BadUnaryUnpackOperator() : base(20, "...") { }

	public override BadExpression Parse(BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadUnaryUnpackExpression(right);
	}
}
