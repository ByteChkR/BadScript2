using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
namespace BadScript2.Parser.Operators.Binary.Math.Atomic;

/// <summary>
///     Implements the Post Increment Operator
/// </summary>
public class BadPostIncrementOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadPostIncrementOperator() : base(2, "++") { }


    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        return new BadPostIncrementExpression(left, left.Position);
    }
}