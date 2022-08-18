using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.ControlFlow;

public class BadReturnExpression : BadExpression
{
    public BadReturnExpression(BadExpression? right, BadSourcePosition position, bool isRefReturn) : base(
        false,
        false,
        position
    )
    {
        Right = right;
        IsRefReturn = isRefReturn;
    }

    private bool IsRefReturn { get; }

    private BadExpression? Right { get; set; }

    public override void Optimize()
    {
        if (Right != null)
        {
            Right = BadExpressionOptimizer.Optimize(Right);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject value = BadObject.Null;
        if (Right == null)
        {
            context.Scope.SetReturnValue(value);

            yield return value;

            yield break;
        }

        foreach (BadObject obj in Right.Execute(context))
        {
            value = obj;

            yield return obj;
        }

        if (!IsRefReturn)
        {
            value = value.Dereference();
        }

        context.Scope.SetReturnValue(value);

        yield return value;
    }

    public override string ToString()
    {
        return "return " + Right ?? "";
    }
}