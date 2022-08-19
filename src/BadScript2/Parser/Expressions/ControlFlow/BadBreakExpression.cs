using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

public class BadBreakExpression : BadExpression
{
    public BadBreakExpression(BadSourcePosition position) : base(false, position) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        context.Scope.SetBreak();

        yield return BadObject.Null;
    }
}