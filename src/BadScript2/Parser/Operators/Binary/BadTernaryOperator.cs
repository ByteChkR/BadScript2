using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Access;
using BadScript2.Reader;

namespace BadScript2.Parser.Operators.Binary;

public class BadTernaryOperator : BadBinaryOperator
{
    public BadTernaryOperator() : base(15, "?") { }

    public override BadExpression Parse(BadExpression left, BadSourceParser parser)
    {
        BadExpression middle = parser.ParseExpression();
        parser.Reader.SkipNonToken();
        parser.Reader.Eat(":");
        parser.Reader.SkipNonToken();
        BadExpression right = parser.ParseExpression();
        parser.Reader.SkipNonToken();

        return new BadTernaryExpression(left, middle, right, left.Position.Combine(right.Position));
    }
}