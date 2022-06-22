using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Parser.Operators.Binary.Logic;

public class BadLogicAndOperator : BadBinaryOperator
{
    public BadLogicAndOperator() : base(13, "&&") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAndExpression(left, right, left.Position.Combine(right.Position));
    }
}