using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Parser.Operators.Binary.Math.Atomic
{
    public class BadPostIncrementOperator : BadBinaryOperator
    {
        public BadPostIncrementOperator() : base(2, "++") { }


        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            return new BadPostIncrementExpression(left, left.Position);
        }
    }
}