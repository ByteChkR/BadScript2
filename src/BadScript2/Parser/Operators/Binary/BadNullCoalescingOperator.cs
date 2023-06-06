using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the Null-Coalescing Operator
/// </summary>
public class BadNullCoalescingOperator : BadBinaryOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadNullCoalescingOperator() : base(15, "??") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression();

		return new BadNullCoalescingExpression(left, right, left.Position.Combine(right.Position));
	}
}
