using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

public class BadPostIncrementExpression : BadExpression
{
    private readonly BadExpression m_Left;

    public BadPostIncrementExpression(BadExpression left, BadSourcePosition position) : base(
        left.IsConstant,
        false,
        position
    )
    {
        m_Left = left;
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in m_Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (left is not BadObjectReference leftRef)
        {
            throw new BadRuntimeException("Left side of ++ must be a reference", Position);
        }

        left = left.Dereference();

        if (left is not IBadNumber leftNumber)
        {
            throw new BadRuntimeException("Left side of ++ must be a number", Position);
        }

        leftRef.Set(leftNumber.Value + 1);

        yield return left;
    }
}