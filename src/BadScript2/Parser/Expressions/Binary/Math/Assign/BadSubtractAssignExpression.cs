using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Assign;

/// <summary>
///     Implements the Subtract Assign Expression
/// </summary>
public class BadSubtractAssignExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Subtract Assignment Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadSubtractAssignExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
                                                                                                                   right,
                                                                                                                   position
                                                                                                                  ) { }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="leftRef">Reference to the left side of the expression</param>
    /// <param name="left">Left side of the expression</param>
    /// <param name="right">Right side of the expression</param>
    /// <param name="position">Position of the expression</param>
    /// <param name="symbol">Symbol of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static BadObject Subtract(BadObjectReference leftRef,
                                     BadObject left,
                                     BadObject right,
                                     BadSourcePosition position,
                                     string symbol)
    {
        if (left is not IBadNumber lNum || right is not IBadNumber rNum)
        {
            throw new BadRuntimeException($"Can not apply operator '{symbol}' to {left} and {right}", position);
        }

        BadObject r = BadObject.Wrap(lNum.Value - rNum.Value);
        leftRef.Set(r);

        return r;
    }

    /// <summary>
    ///     Executes the Operator with operator override
    /// </summary>
    /// <param name="context">The caller.</param>
    /// <param name="leftRef">Reference to the left side of the expression</param>
    /// <param name="right">Right side of the expression</param>
    /// <param name="position">Position of the expression</param>
    /// <param name="symbol">Symbol of the expression</param>
    /// <returns>Returns the result of the operation.(Last Item)</returns>
    public static IEnumerable<BadObject> SubtractWithOverride(BadExecutionContext? context,
                                                              BadObjectReference leftRef,
                                                              BadObject right,
                                                              BadSourcePosition position,
                                                              string symbol)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.SUBTRACT_ASSIGN_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(left,
                                                            right,
                                                            context!,
                                                            BadStaticKeys.SUBTRACT_ASSIGN_OPERATOR_NAME,
                                                            position
                                                           ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Subtract(leftRef, left, right, position, symbol);
        }
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
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

        foreach (BadObject o in SubtractWithOverride(context, leftRef, right, Position, GetSymbol()))
        {
            yield return o;
        }
    }

    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "-=";
    }
}