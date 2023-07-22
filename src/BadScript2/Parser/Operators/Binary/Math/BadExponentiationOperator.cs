using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math;

public class BadExponentiationOperator : BadBinaryOperator
{
    public BadExponentiationOperator() : base(4, "**") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadExponentiationExpression(left, right, left.Position.Combine(right.Position));
    }
}