using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

/// <summary>
/// Contains the Atomic Math Expressions for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Post Decrement Expression
/// </summary>
public class BadPostDecrementExpression : BadExpression
{
    /// <summary>
    ///     Left side of the expression
    /// </summary>
    public readonly BadExpression Left;

    /// <summary>
    ///     Constructor of the Post Decrement Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPostDecrementExpression(BadExpression left, BadSourcePosition position) : base(left.IsConstant,
                                                                                             position
                                                                                            )
    {
        Left = left;
    }


    /// <inheritdoc cref="BadExpression.GetDescendants" />
    public override IEnumerable<BadExpression> GetDescendants()
    {
        return Left.GetDescendantsAndSelf();
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
        BadObject value = reference.Dereference(position);

        if (value is not IBadNumber leftNumber)
        {
            throw new BadRuntimeException("Left side of -- must be a number", position);
        }

        reference.Set(leftNumber.Value - 1, position);

        return value;
    }

    /// <summary>
    ///     Executes the Operator
    /// </summary>
    /// <param name="context">The caller.</param>
    /// <param name="leftRef">Reference to the left side of the expression</param>
    /// <param name="position">Position of the expression</param>
    /// <returns>Returns the result of the operation</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be applied</exception>
    public static IEnumerable<BadObject> DecrementWithOverride(BadExecutionContext? context,
                                                               BadObjectReference leftRef,
                                                               BadSourcePosition position)
    {
        BadObject left = leftRef.Dereference(position);

        if (left.HasProperty(BadStaticKeys.POST_DECREMENT_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(left,
                                                            context!,
                                                            BadStaticKeys.POST_DECREMENT_OPERATOR_NAME,
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
            throw new BadRuntimeException("Left side of -- must be a reference", Position);
        }

        foreach (BadObject o in DecrementWithOverride(context, leftRef, Position))
        {
            yield return o;
        }
    }
}