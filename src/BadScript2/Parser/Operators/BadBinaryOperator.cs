using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators;

public abstract class BadBinaryOperator : BadOperator
{
    protected BadBinaryOperator(int precedence, string symbol) : base(precedence, symbol) { }

    public abstract BadExpression Parse(BadExpression left, BadSourceParser parser);
}