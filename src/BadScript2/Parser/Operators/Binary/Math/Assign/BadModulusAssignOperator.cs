using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Parser.Operators.Binary.Math.Assign;

public class BadModulusAssignOperator : BadBinaryOperator
{
    public BadModulusAssignOperator() : base(15, "%=") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression right = parser.ParseExpression(null, Precedence);

        return new BadModulusAssignExpression(left, right, left.Position.Combine(right.Position));
    }
}