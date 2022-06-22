using BadScript2.Common;

namespace BadScript2.Parser.Expressions.Constant
{
    public class BadBooleanExpression : BadConstantExpression<bool>
    {
        public BadBooleanExpression(bool value, BadSourcePosition position) : base(value, position) { }
    }
}