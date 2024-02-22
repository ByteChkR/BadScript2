using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Reader;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the '...' operator.
/// </summary>
public class BadBinaryUnpackOperator : BadBinaryOperator
{
	/// <summary>
	///     Creates a new '...' operator
	/// </summary>
	public BadBinaryUnpackOperator() : base(3, "...", false) { }

    /// <inheritdoc cref="BadBinaryOperator.Parse" />
    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        parser.Reader.SkipNonToken();
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadBinaryUnpackExpression(left, right, left.Position.Combine(right.Position));
    }
}