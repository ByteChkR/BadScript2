using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math;

/// <summary>
///     Implements the '**' Operator
/// </summary>
public class BadExponentiationOperator : BadBinaryOperator
{
	/// <summary>
	///     Creates a new '**' Operator
	/// </summary>
	public BadExponentiationOperator() : base(4, "**") { }

	/// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadExponentiationExpression(left, right, left.Position.Combine(right.Position));
    }
}