using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Constant;

public class BadNullExpression : BadExpression, IBadNativeExpression
{
    public BadNullExpression(BadSourcePosition position) : base(true, false, position) { }

    public override string ToString()
    {
        return BadStaticKeys.Null;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        yield return BadObject.Null;
    }
}