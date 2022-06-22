using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Operators
{
    public abstract class BadUnaryPrefixOperator : BadOperator
    {
        protected BadUnaryPrefixOperator(int precedence, string symbol) : base(precedence, symbol) { }
        public abstract BadExpression Parse(BadSourceParser parser);
    }
}