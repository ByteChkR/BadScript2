using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Parser.Operators.Binary;

/// <summary>
///     Implements the Range Operator
/// </summary>
public class BadRangeOperator : BadBinaryOperator
{
	/// <summary>
	///     Constructor of the Operator
	/// </summary>
	public BadRangeOperator() : base(15, "..") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadRangeExpression(left, right, left.Position.Combine(right.Position));
    }
}