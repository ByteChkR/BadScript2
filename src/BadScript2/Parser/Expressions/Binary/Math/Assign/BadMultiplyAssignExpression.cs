using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Assign;

/// <summary>
///     Implements the Multiply Assignment Expression
/// </summary>
public class BadMultiplyAssignExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Multiply Assignment Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadMultiplyAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    public static BadObject Multiply(
        BadObjectReference leftRef,
        BadObject left,
        BadObject right,
        BadSourcePosition position,
        string symbol)
    {
        if (left is not IBadNumber lNum || right is not IBadNumber rNum)
        {
            throw new BadRuntimeException($"Can not apply operator '{symbol}' to {left} and {right}", position);
        }

        BadObject r = BadObject.Wrap(lNum.Value * rNum.Value);
        leftRef.Set(r);

        return r;
    }

    public static IEnumerable<BadObject> MultiplyWithOverride(
        BadExecutionContext? context,
        BadObjectReference leftRef,
        BadObject right,
        BadSourcePosition position,
        string symbol)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.MULTIPLY_ASSIGN_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context!,
                         BadStaticKeys.MULTIPLY_ASSIGN_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Multiply(leftRef, left, right, position, symbol);
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

        foreach (BadObject o in MultiplyWithOverride(context, leftRef, right, Position, GetSymbol()))
        {
            yield return o;
        }
    }

    protected override string GetSymbol()
    {
        return "*=";
    }
}