using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Parser.Operators.Binary.Math.Atomic;

/// <summary>
///     Implements the Pre Increment Operator
/// </summary>
public class BadPreIncrementOperator : BadUnaryPrefixOperator
{
    /// <summary>
    ///     Constructor of the Operator
    /// </summary>
    public BadPreIncrementOperator() : base(2, "++") { }


	public override BadExpression Parse(BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadPreIncrementExpression(right, right.Position);
	}
}
