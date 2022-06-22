using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Parser.Operators.Binary
{
    public class BadNullCoalescingAssignOperator : BadBinaryOperator
    {
        public BadNullCoalescingAssignOperator() : base(15, "??=") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression();

            return new BadNullCoalescingAssignExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}