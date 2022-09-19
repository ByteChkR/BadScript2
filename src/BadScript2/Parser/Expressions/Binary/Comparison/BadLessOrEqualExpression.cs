using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Comparison;


/// <summary>
/// Implements the Less or Equal Expression
/// <Left> <= <Right>
/// </summary>
public class BadLessOrEqualExpression : BadBinaryExpression
{
    
    /// <summary>
    /// Constructor of the Less or Equal Expression
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
    /// Returns true if the left side is less or equal to the right side
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
    /// <returns>True if the Left side is less or equal than the right side. Otherwise false.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
    public static BadObject LessOrEqual(BadObject left, BadObject right, BadSourcePosition pos)
    {
        if (left is IBadNumber lNum)
        {
            if (right is IBadNumber rNum)
            {
                if (lNum.Value <= rNum.Value)
                {
                    return BadObject.True;
                }

                return BadObject.False;
            }
        }

        throw new BadRuntimeException($"Can not apply operator '<=' to {left} and {right}", pos);
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

        yield return LessOrEqual(left, right, Position);
    }

    protected override string GetSymbol()
    {
        return "<=";
    }
}