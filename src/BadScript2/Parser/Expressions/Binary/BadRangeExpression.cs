using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary;

/// <summary>
///     Implements the Range Expression
///     <Start>..<End>
/// </summary>
public class BadRangeExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Range Expression
    /// </summary>
    /// <param name="left">Start of the Range</param>
    /// <param name="right">End of the Range</param>
    /// <param name="position">Source position of the Expression</param>
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

    /// <summary>
    ///     Returns a range of numbers
    /// </summary>
    /// <param name="from">Start of the Range</param>
    /// <param name="to">End of the Range(Exclusive></param>
    /// <returns></returns>
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