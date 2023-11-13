using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Parser.Operators.Binary.Math.Assign;

/// <summary>
///     Implements the Divide Assign Operator
/// </summary>
public class BadDivideAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadDivideAssignOperator() : base(16, "/=", false) { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadDivideAssignExpression(left, right, left.Position.Combine(right.Position));
	}
}
