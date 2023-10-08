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
        if (left is IBadString lStr)
        {
            if (right is IBadNative rNative)
            {
                return BadObject.Wrap(lStr.Value + rNative.Value);
            }

            return BadObject.Wrap(lStr.Value + right);
        }

        if (left is IBadNumber lNum)
        {
            if (right is IBadString rStr)
            {
                return BadObject.Wrap(lNum.Value + rStr.Value);
            }

            if (right is IBadNumber rNum)
            {
                return BadObject.Wrap(lNum.Value + rNum.Value);
            }
        }
        else if (left is IBadBoolean lBool)
        {
            if (right is IBadString rStr)
            {
                return BadObject.Wrap(lBool.Value + rStr.Value);
            }
        }
        else if (right is IBadString rStr)
        {
            return BadObject.Wrap(left + rStr.Value);
        }

        throw new BadRuntimeException($"Can not apply operator '+' to {left} and {right}", pos);
    }


    public static IEnumerable<BadObject> AddWithOverride(
        BadExecutionContext context,
        BadObject leftRef,
        BadObject right,
        BadSourcePosition position)
    {
        BadObject left = leftRef.Dereference();

        if (left.HasProperty(BadStaticKeys.AddOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         context,
                         BadStaticKeys.AddOperatorName,
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