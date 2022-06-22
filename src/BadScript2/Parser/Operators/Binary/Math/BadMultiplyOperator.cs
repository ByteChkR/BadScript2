using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math
{
    public class BadMultiplyOperator : BadBinaryOperator
    {
        public BadMultiplyOperator() : base(5, "*") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadMultiplyExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}