using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Parser.Operators.Binary;

public class BadInOperator : BadBinaryOperator
{
    public BadInOperator() : base(15, "in") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadInExpression(left, right, left.Position.Combine(right.Position));
    }
}