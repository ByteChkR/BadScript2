using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;
namespace BadScript2.Parser.Operators.Binary.Math.Atomic;

/// <summary>
///     Implements the Pre Decrement Operator
/// </summary>
public class BadPreDecrementOperator : BadUnaryPrefixOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadPreDecrementOperator() : base(2, "--", false) { }


    /// <inheritdoc cref="BadUnaryPrefixOperator.Parse" />
    public override BadExpression Parse(BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadPreDecrementExpression(right, right.Position);
    }
}