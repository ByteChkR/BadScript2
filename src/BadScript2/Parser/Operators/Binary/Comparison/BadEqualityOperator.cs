using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison;

/// <summary>
///     Implements the Equality Operator
/// </summary>
public class BadEqualityOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadEqualityOperator() : base(9, "==") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadEqualityExpression(left, right, left.Position.Combine(right.Position));
	}
}
