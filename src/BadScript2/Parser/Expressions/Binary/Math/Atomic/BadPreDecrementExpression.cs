using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Pre Decrement Expression
/// </summary>
public class BadPreDecrementExpression : BadExpression
{
    /// <summary>
    ///     Right side of the Expression
    /// </summary>
    public readonly BadExpression Right;

    /// <summary>
    ///     Constructor of the Pre Decrement Expression
    /// </summary>
    /// <param name="right">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPreDecrementExpression(BadExpression right, BadSourcePosition position) : base(
        right.IsConstant,
        position
    )
    {
        Right = right;
    }

    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Right.GetDescendantsAndSelf();
    }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="reference">Reference to the left side of the expression</param>
    /// <param name="position">Position of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static BadObject Decrement(BadObjectReference reference, BadSourcePosition position)
    {
        BadObject right = reference.Dereference();

        if (right is not IBadNumber leftNumber)
        {
            throw new BadRuntimeException("Right side of -- must be a number", position);
        }

        BadObject r = leftNumber.Value - 1;
        reference.Set(r);

        return r;
    }

    /// <inheritdoc cref="BadExpression.InnerExecute" />
    protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
    {
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        if (right is not BadObjectReference rightRef)
        {
            throw new BadRuntimeException("Right side of -- must be a reference", Position);
        }

        foreach (BadObject o in DecrementWithOverride(context, rightRef, Position))
        {
            yield return o;
        }
    }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="context">The caller.</param>
    /// <param name="leftRef">Reference to the left side of the expression</param>
    /// <param name="position">Position of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static IEnumerable<BadObject> DecrementWithOverride(
        BadExecutionContext? context,
        BadObjectReference leftRef,
        BadSourcePosition position)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.PRE_DECREMENT_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         context!,
                         BadStaticKeys.PRE_DECREMENT_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Decrement(leftRef, position);
        }
    }
}