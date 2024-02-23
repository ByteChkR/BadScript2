using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Less or Equal Expression
///     LEFT &lt;= RIGHT
/// </summary>
public class BadLessOrEqualExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Less or Equal Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadLessOrEqualExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    /// <summary>
    ///     Returns true if the left side is less or equal to the right side
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
    /// <returns>True if the Left side is less or equal than the right side. Otherwise false.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
    public static BadObject LessOrEqual(BadObject left, BadObject right, BadSourcePosition pos)
    {
        if (left is not IBadNumber lNum)
        {
            throw new BadRuntimeException($"Can not apply operator '<=' to {left} and {right}", pos);
        }

        if (right is IBadNumber rNum)
        {
            return lNum.Value <= rNum.Value ? BadObject.True : BadObject.False;
        }

        throw new BadRuntimeException($"Can not apply operator '<=' to {left} and {right}", pos);
    }

    /// <summary>
    ///     Executes the expression
    /// </summary>
    /// <param name="context">The caller.</param>
    /// <param name="left">The left.</param>
    /// <param name="right">The right.</param>
    /// <param name="position">The position.</param>
    /// <returns>Result of the operator override.(last item)</returns>
    public static IEnumerable<BadObject> LessOrEqualWithOverride(
        BadExecutionContext? context,
        BadObject left,
        BadObject right,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context!,
                         BadStaticKeys.LESS_EQUAL_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else if (right.HasProperty(BadStaticKeys.LESS_EQUAL_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         right,
                         left,
                         context!,
                         BadStaticKeys.LESS_EQUAL_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else

        {
            yield return LessOrEqual(left, right, position);
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

        left = left.Dereference();
        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();


        foreach (BadObject o in LessOrEqualWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "<=";
    }
}