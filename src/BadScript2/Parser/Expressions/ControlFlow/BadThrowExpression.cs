using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

public class BadThrowExpression : BadExpression
{
    public BadThrowExpression(BadExpression right, BadSourcePosition position) : base(
        false,
        position
    )
    {
        Right = right;
    }

    private BadExpression Right { get; set; }

    public override void Optimize()
    {
        Right = BadExpressionOptimizer.Optimize(Right);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject value = BadObject.Null;

        foreach (BadObject obj in Right.Execute(context))
        {
            value = obj;

            yield return obj;
        }

        value = value.Dereference();

        context.Scope.SetError(value, null);

        yield return value;
    }
}