using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Parser.Operators.Binary
{
    public class BadAssignOperator : BadBinaryOperator
    {
        public BadAssignOperator() : base(15, "=") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadAssignExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}