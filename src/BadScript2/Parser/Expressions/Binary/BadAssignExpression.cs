using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary;

public class BadAssignExpression : BadExpression
{
    public BadAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        false,
        true,
        position
    )
    {
        Left = left;
        Right = right;
    }

    public BadExpression Left { get; private set; }
    public BadExpression Right { get; private set; }

    public override void Optimize()
    {
        Left = BadExpressionOptimizer.Optimize(Left);
        Right = BadExpressionOptimizer.Optimize(Right);
    }

    public override string ToString()
    {
        return $"({Left} = {Right})";
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (context.Scope.IsError)
        {
            yield break;
        }

        BadObject right = BadObject.Null;
        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        if (context.Scope.IsError)
        {
            yield break;
        }

        right = right.Dereference();

        if (left is BadObjectReference reference)
        {
            reference.Set(right);
        }
        else
        {
            throw new BadRuntimeException($"Left handside of {this} is not a reference", Position);
        }

        yield return left;
    }
}