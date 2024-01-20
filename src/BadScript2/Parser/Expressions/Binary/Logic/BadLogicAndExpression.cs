using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic;

/// <summary>
///     Implements the Logic And Expression
/// </summary>
public class BadLogicAndExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Logic And Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadLogicAndExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

    /// <summary>
    ///     Returns true if left and right are true
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a boolean</param>
    /// <returns>True if the Left side and the right side are true. Otherwise false.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadBoolean</exception>
    public static BadObject And(BadObject left, BadObject right, BadSourcePosition pos)
    {
        if (left is not IBadBoolean lBool)
        {
            throw new BadRuntimeException($"Can not apply operator '&&' to {left}. expected boolean", pos);
        }

        if (!lBool.Value)
        {
            return BadObject.False;
        }

        if (right is IBadBoolean rBool)
        {
            return rBool.Value ? BadObject.True : BadObject.False;
        }

        throw new BadRuntimeException($"Can not apply operator '&&' to {left} and {right}", pos);
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

        if (left is not IBadBoolean lBool)
        {
            throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left}. expected boolean", Position);
        }

        if (!lBool.Value)
            {
                yield return BadObject.False;

                yield break;
            }

        BadObject right = BadObject.Null;

        foreach (BadObject o in Right.Execute(context))
        {
            right = o;

            yield return o;
        }

        right = right.Dereference();

        if (right is not IBadBoolean rBool)
        {
            throw new BadRuntimeException($"Can not apply operator '{GetSymbol()}' to {left} and {right}", Position);
        }

        if (rBool.Value)
            {
                yield return BadObject.True;
            }
            else
            {
                yield return BadObject.False;
            }
    }

    protected override string GetSymbol()
    {
        return "&&";
    }
}