using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Parser.Operators.Binary.Logic;

public class BadLogicNotOperator : BadUnaryPrefixOperator
{
    public BadLogicNotOperator() : base(3, "!") { }

    public override BadExpression Parse(BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicNotExpression(right, right.Position);
    }
}