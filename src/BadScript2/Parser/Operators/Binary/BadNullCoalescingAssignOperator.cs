using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the Null-Coalescing Assign Operator
/// </summary>
public class BadNullCoalescingAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadNullCoalescingAssignOperator() : base(16, "??=", false) { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression();

        return new BadNullCoalescingAssignExpression(left, right, left.Position.Combine(right.Position));
    }
}