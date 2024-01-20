using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Inequality Expression
/// </summary>
public class BadInequalityExpression : BadBinaryExpression
{
	/// <summary>
	///     Constructor for the Inequality Expression
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source position of the Expression</param>
	public BadInequalityExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(
        left,
        right,
        position
    ) { }

	/// <summary>
	///     Returns True if the two Objects are not equal
	/// </summary>
	/// <param name="left">Left Object</param>
	/// <param name="right">Right Object</param>
	/// <returns>True if the Objects are not Equal. False Otherwise</returns>
	public static BadObject NotEqual(BadObject left, BadObject right)
    {
        return left.Equals(right) ? BadObject.False : BadObject.True;
    }

    public static IEnumerable<BadObject> NotEqualWithOverride(
        BadExecutionContext? caller,
        BadObject left,
        BadObject right,
        BadSourcePosition position)
    {
        if (left.HasProperty(BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, caller?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         left,
                         right,
                         caller!,
                         BadStaticKeys.NOT_EQUAL_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else if (right.HasProperty(BadStaticKeys.NOT_EQUAL_OPERATOR_NAME, caller?.Scope))
        {
            foreach (BadObject o in ExecuteOperatorOverride(
                         right,
                         left,
                         caller!,
                         BadStaticKeys.NOT_EQUAL_OPERATOR_NAME,
                         position
                     ))
            {
                yield return o;
            }
        }
        else

        {
            yield return NotEqual(left, right);
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

        foreach (BadObject? o in NotEqualWithOverride(context, left, right, Position))
        {
            yield return o;
        }
    }

    protected override string GetSymbol()
    {
        return "!=";
    }
}