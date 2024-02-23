using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math;

/// <summary>
///     Implements the Divide Operator
/// </summary>
public class BadDivideOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadDivideOperator() : base(5, "/") { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadDivideExpression(left, right, left.Position.Combine(right.Position));
    }
}