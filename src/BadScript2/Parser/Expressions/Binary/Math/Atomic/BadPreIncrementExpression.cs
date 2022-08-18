using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

public class BadPreIncrementExpression : BadExpression
{
    private readonly BadExpression m_Right;

    public BadPreIncrementExpression(BadExpression right, BadSourcePosition position) : base(
        right.IsConstant,
        false,
        position
    )
    {
        m_Right = right;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject right = BadObject.Null;
        foreach (BadObject o in m_Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        if (right is not BadObjectReference rightRef)
        {
            throw new BadRuntimeException("Right side of ++ must be a reference", Position);
        }

        right = right.Dereference();

        if (right is not IBadNumber leftNumber)
        {
            throw new BadRuntimeException("Right side of ++ must be a number", Position);
        }

        BadObject r = leftNumber.Value + 1;
        rightRef.Set(r);

        yield return r;
    }
}