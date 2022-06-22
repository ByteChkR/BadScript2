using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Parser.Operators.Binary.Logic.Assign;

public class BadLogicAssignAndOperator : BadBinaryOperator
{
    public BadLogicAssignAndOperator() : base(15, "&=") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadLogicAssignAndExpression(left, right, left.Position.Combine(right.Position));
    }
}