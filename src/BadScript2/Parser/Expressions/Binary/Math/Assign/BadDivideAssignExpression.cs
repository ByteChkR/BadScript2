using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Assign;

/// <summary>
///     Implements the Divide Assignment Expression
/// </summary>
public class BadDivideAssignExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Divide Assignment Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadDivideAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    public static BadObject Divide(BadObjectReference leftRef, BadObject left, BadObject right, BadSourcePosition position, string symbol)
    {
        if (left is IBadNumber lNum && right is IBadNumber rNum)
        {
            BadObject r = BadObject.Wrap(lNum.Value / rNum.Value);
            leftRef.Set(r);

            return r;
        }

        throw new BadRuntimeException($"Can not apply operator '{symbol}' to {left} and {right}", position);
    }

    public static IEnumerable<BadObject> DivideWithOverride(BadExecutionContext context, BadObjectReference leftRef, BadObject right, BadSourcePosition position, string symbol)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.DivideAssignOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context,
                         BadStaticKeys.DivideAssignOperatorName,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Divide(leftRef, left, right, position, symbol);
        }
    }

    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject left = BadObject.Null;
        foreach (BadObject o in Left.Execute(context))
        {
            left = o;

            yield return o;
        }

        if (left is not BadObjectReference leftRef)
        {
            throw new BadRuntimeException($"Left side of {GetSymbol()} must be a reference", Position);
        }

        BadObject right = BadObject.Null;
        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        foreach (BadObject o in DivideWithOverride(context, leftRef, right, Position, GetSymbol()))
        {
            yield return o;
        }
    }

    protected override string GetSymbol()
    {
        return "/=";
    }
}