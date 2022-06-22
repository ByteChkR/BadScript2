using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Reader;
using BadScript2.Reader.Token;

namespace BadScript2.Parser.Operators.Binary
{
    public class BadMemberAccessOperator : BadBinaryOperator
    {
        public BadMemberAccessOperator() : base(2, ".") { }

        public override BadExpression Parse(BadExpression left, BadSourceParser parser)
        {
            BadWordToken right = parser.Reader.ParseWord();

            return new BadMemberAccessExpression(left, right, left.Position.Combine(right.SourcePosition));
        }
    }
}