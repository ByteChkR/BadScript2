using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Parser.Operators.Binary.Math.Assign;

/// <summary>
///     Implements the '**=' operator.
/// </summary>
public class BadExponentiationAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Creates a new '**=' operator
	/// </summary>
	public BadExponentiationAssignOperator() : base(16, "**=", false) { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadExponentiationAssignExpression(left, right, left.Position.Combine(right.Position));
	}
}
