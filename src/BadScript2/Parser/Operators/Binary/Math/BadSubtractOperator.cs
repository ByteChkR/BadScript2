using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Parser.Operators.Binary.Math
{
    public class BadSubtractOperator : BadBinaryOperator
    {
        public BadSubtractOperator() : base(6, "-") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadSubtractExpression(left, right, left.Position.Combine(right.Position));
        }
    }
}