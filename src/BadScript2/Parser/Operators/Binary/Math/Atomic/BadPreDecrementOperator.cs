using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Parser.Operators.Binary.Math.Atomic
{
    public class BadPreDecrementOperator : BadUnaryPrefixOperator
    {
        public BadPreDecrementOperator() : base(2, "--") { }


        public override BadExpression Parse(BadSourceParser parser)
        {
            BadExpression right = parser.ParseExpression(null, Precedence);

            return new BadPreDecrementExpression(right, right.Position);
        }
    }
}