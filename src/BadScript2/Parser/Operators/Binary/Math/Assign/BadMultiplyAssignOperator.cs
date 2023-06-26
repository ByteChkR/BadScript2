using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Parser.Operators.Binary.Math.Assign;

/// <summary>
///     Implements the Multiply Assign Operator
/// </summary>
public class BadMultiplyAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadMultiplyAssignOperator() : base(15, "*=") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadMultiplyAssignExpression(left, right, left.Position.Combine(right.Position));
	}
}