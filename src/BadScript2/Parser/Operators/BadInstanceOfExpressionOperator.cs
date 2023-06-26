using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

public class BadInstanceOfExpressionOperator : BadBinaryOperator
{
	public BadInstanceOfExpressionOperator() : base(3, "instanceof") { }

	public override BadExpression Parse(BadExpression left, BadSourceParser parser)
	{
		BadExpression right = parser.ParseExpression(null, Precedence);

		return new BadInstanceOfExpression(left, right, left.Position.Combine(right.Position));
	}
}