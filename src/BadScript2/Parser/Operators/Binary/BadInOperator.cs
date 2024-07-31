using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the 'in' operator.
/// </summary>
public class BadInOperator : BadBinaryOperator
{
	/// <summary>
	///     Creates a new 'in' operator
	/// </summary>
	public BadInOperator() : base(3, "in") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadInExpression(left, right, left.Position.Combine(right.Position));
    }
}