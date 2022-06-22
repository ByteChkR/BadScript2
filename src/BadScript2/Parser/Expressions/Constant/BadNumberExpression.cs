using BadScript2.Common;

namespace BadScript2.Parser.Expressions.Constant
{
    public class BadNumberExpression : BadConstantExpression<decimal>
    {
        public BadNumberExpression(decimal value, BadSourcePosition position) : base(value, position) { }
    }
}