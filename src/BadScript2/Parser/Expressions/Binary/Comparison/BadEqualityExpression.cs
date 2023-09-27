using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Equality Expression
///     <Left> == <Right>
/// </summary>
public class BadEqualityExpression : BadBinaryExpression
{
	/// <summary>
	///     Constructor of the Equality Expression
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source Position of the Expression</param>
	public BadEqualityExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

	/// <summary>
	///     Returns true if the left side is equal to the right side.
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <returns>True if left is equal to right</returns>
	public static BadObject Equal(BadObject left, BadObject right)
    {
        if (left.Equals(right))
        {
            return BadObject.True;
        }

        return BadObject.False;
    }

    public static IEnumerable<BadObject> EqualWithOverride(
        BadExecutionContext caller,
        BadObject left,
        BadObject right,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.EqualOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         caller,
                         BadStaticKeys.EqualOperatorName,
                         position
                     ))
            {
                yield return o;
            }
        }
        else if (right.HasProperty(BadStaticKeys.EqualOperatorName))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         right,
                         left,
                         caller,
                         BadStaticKeys.EqualOperatorName,
                         position
                     ))
            {
                yield return o;
            }
        }
        else
        {
            yield return Equal(left, right);
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

        foreach (BadObject o in EqualWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    protected override string GetSymbol()
    {
        return "==";
    }
}