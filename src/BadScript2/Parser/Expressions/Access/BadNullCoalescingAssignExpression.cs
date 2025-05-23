using BadScript2.Common;
using BadScript2.Parser.Expressions.Binary;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Access;

/// <summary>
///     Implements the Null Coalescing Assign Expression
///     LEFT ??= RIGHT
/// </summary>
public class BadNullCoalescingAssignExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Null Coalescing Assign Expression
    /// </summary>
    /// <param name="left">Left side of the expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Position inside the source code</param>
    public BadNullCoalescingAssignExpression(BadExpression left,
                                             BadExpression right,
                                             BadSourcePosition position) : base(left,
                                                                                right,
                                                                                position
                                                                               ) { }


    /// <inheritdoc />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;

        foreach (BadObject o in Left.Execute(context))
        {
            left = o;
        }

        if (left is not BadObjectReference leftRef)
        {
            throw new BadRuntimeException("Left side of null coalescing assignment must be a reference", Position);
        }

        left = left.Dereference(Position);

        if (left == BadObject.Null)
        {
            BadObject rVal = BadObject.Null;

            foreach (BadObject o in Right.Execute(context))
            {
                yield return o;

                rVal = o;
            }

            rVal = rVal.Dereference(Position);
            leftRef.Set(rVal, Position);

            yield return rVal;
        }
        else
        {
            yield return left;
        }
    }


    /// <inheritdoc />
    protected override string GetSymbol()
    {
        return "??=";
    }
}