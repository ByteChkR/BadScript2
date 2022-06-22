using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

public class BadContinueExpression : BadExpression
{
    public BadContinueExpression(BadSourcePosition position) : base(false, false, position) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        context.Scope.SetContinue();

        yield return BadObject.Null;
    }
}