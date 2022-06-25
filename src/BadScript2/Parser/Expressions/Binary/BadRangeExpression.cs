using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary;

public class BadRangeExpression : BadBinaryExpression
{
    public BadRangeExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        BadObject right = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        left = left.Dereference();
        right = right.Dereference();

        if (left is not IBadNumber lNum)
        {
            throw new BadRuntimeException("Left side of range operator is not a number", Position);
        }

        if (right is not IBadNumber rNum)
        {
            throw new BadRuntimeException("Right side of range operator is not a number", Position);
        }

        if (lNum.Value > rNum.Value)
        {
            throw new BadRuntimeException("Left side of range operator is greater than right side", Position);
        }

        yield return new BadInteropEnumerator(Range(lNum.Value, rNum.Value).GetEnumerator());
    }

    public static IEnumerable<BadObject> Range(decimal from, decimal to)
    {
        for (decimal i = from; i < to; i++)
        {
            yield return BadObject.Wrap(i);
        }
    }

    protected override string GetSymbol()
    {
        return "..";
    }
}