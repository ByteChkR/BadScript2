using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant
{
    public class BadStringExpression : BadConstantExpression<string>
    {
        public BadStringExpression(string value, BadSourcePosition position) : base(value, position) { }

        protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
        {
            yield return BadObject.Wrap(Value.Substring(1, Value.Length - 2));
        }
    }
}