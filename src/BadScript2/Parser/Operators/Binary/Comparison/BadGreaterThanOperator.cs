using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison;

/// <summary>
///     Implements the Greater Than Operator
/// </summary>
public class BadGreaterThanOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadGreaterThanOperator() : base(8, ">") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadGreaterThanExpression(left, right, left.Position.Combine(right.Position));
	}
}
