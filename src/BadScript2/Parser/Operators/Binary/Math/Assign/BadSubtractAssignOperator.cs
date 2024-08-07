using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Assign;
namespace BadScript2.Parser.Operators.Binary.Math.Assign;

/// <summary>
///     Implements the Subtract Assign Operator
/// </summary>
public class BadSubtractAssignOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadSubtractAssignOperator() : base(16, "-=", false) { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadSubtractAssignExpression(left, right, left.Position.Combine(right.Position));
    }
}