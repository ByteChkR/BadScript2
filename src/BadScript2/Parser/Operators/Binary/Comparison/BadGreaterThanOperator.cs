using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Parser.Operators.Binary.Comparison;

public class BadGreaterThanOperator : BadBinaryOperator
{
    public BadGreaterThanOperator() : base(8, ">") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadGreaterThanExpression(left, right, left.Position.Combine(right.Position));
    }
}