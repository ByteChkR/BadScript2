using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison;

/// <summary>
///     Implements the Less or Equal Operator
/// </summary>
public class BadLessOrEqualOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadLessOrEqualOperator() : base(8, "<=") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadLessOrEqualExpression(left, right, left.Position.Combine(right.Position));
	}
}
