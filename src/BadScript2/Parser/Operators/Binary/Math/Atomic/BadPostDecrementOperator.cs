using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Parser.Operators.Binary.Math.Atomic;

/// <summary>
///     Implements the Post Decrement Operator
/// </summary>
public class BadPostDecrementOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadPostDecrementOperator() : base(2, "--") { }


	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		return new BadPostDecrementExpression(left, left.Position);
	}
}
