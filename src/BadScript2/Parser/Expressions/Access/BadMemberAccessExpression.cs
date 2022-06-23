using BadScript2.Common;
using BadScript2.Optimizations;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access;

public class BadMemberAccessExpression : BadExpression
{
    public readonly bool NullChecked;

    public BadMemberAccessExpression(
        BadExpression left,
        BadWordToken right,
        BadSourcePosition position,
        bool nullChecked = false) : base(
        false,
        true,
        position
    )
    {
        Left = left;
        Right = right;
        NullChecked = nullChecked;
    }

    public BadExpression Left { get; private set; }
    public BadWordToken Right { get; }

    public override void Optimize()
    {
        Left = BadExpressionOptimizer.Optimize(Left);
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        left = left.Dereference();

        if (NullChecked && left.Equals(BadObject.Null))
        {
            yield return BadObject.Null;
        }
        else
        {
            yield return left.GetProperty(BadObject.Wrap(Right.Text));
        }
    }


    public override string ToString()
    {
        return $"({Left}{(NullChecked ? "?" : "")}.{Right})";
    }
}