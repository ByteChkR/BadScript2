using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

/// <summary>
///     Implements the Instance Of Operator
/// </summary>
public class BadInstanceOfExpressionOperator : BadBinaryOperator
{
	/// <summary>
	///     Creates a new Instance Of Operator
	/// </summary>
	public BadInstanceOfExpressionOperator() : base(3, "instanceof") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadInstanceOfExpression(left, right, left.Position.Combine(right.Position));
    }
}