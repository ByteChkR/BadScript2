using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math;

/// <summary>
///     Implements the Add Expression
/// </summary>
public class BadAddExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Add Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadAddExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    
    /// <inheritdoc cref="BadBinaryExpression.GetSymbol" />
    protected override string GetSymbol()
    {
        return "+";
    }

    /// <summary>
    ///     Adds left and right together
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right can not be added</param>
    /// <returns>The result of the addition of left and right</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side can not be added</exception>
    public static BadObject Add(BadObject left, BadObject right, BadSourcePosition pos)
    {
        switch (left)
        {
            case IBadString lStr when right is IBadNative rNative:
                return BadObject.Wrap(lStr.Value + rNative.Value);
            case IBadString lStr:
                return BadObject.Wrap(lStr.Value + right);
            case IBadNumber lNum when right is IBadString rStr:
                return BadObject.Wrap(lNum.Value + rStr.Value);
            case IBadNumber lNum when right is IBadNumber rNum:
                return BadObject.Wrap(lNum.Value + rNum.Value);
            case IBadNumber:
                break;
            case IBadBoolean lBool:
            {
                if (right is IBadString rStr)
                {
                    return BadObject.Wrap(lBool.Value + rStr.Value);
                }

                break;
            }
            default:
            {
                if (right is IBadString rStr)
                {
                    return BadObject.Wrap(left + rStr.Value);
                }

                break;
            }
        }

        throw new BadRuntimeException($"Can not apply operator '+' to {left} and {right}", pos);
    }

    /// <summary>
    ///     Executes the Operator with operator overrides
    /// </summary>
    /// <param name="context">The current Script Execution Context</param>
    /// <param name="leftRef">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source position that is used to generate an Exception if the operator can not be computed</param>
    /// <returns>The result of the operator</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the operator can not be executed</exception>
    public static IEnumerable<BadObject> AddWithOverride(
        BadExecutionContext? context,
        BadObject leftRef,
        BadObject right,
        BadSourcePosition position)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.ADD_OPERATOR_NAME, context?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context!,
                         BadStaticKeys.ADD_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Add(left, right, position);
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

        foreach (BadObject? o in AddWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }
}