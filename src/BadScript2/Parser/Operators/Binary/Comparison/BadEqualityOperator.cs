using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
/// Contains the Comparison Operators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Operators.Binary.Comparison;

/// <summary>
///     Implements the Equality Operator
/// </summary>
public class BadEqualityOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadEqualityOperator() : base(10, "==") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadEqualityExpression(left, right, left.Position.Combine(right.Position));
    }
}