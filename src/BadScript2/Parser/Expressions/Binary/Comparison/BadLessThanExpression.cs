using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Less Than Expression
/// </summary>
public class BadLessThanExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Greater Than Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadLessThanExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

    /// <summary>
    ///     Returns true if the left side is less than the right side
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
    /// <returns>True if the Left side is less than the right side. Otherwise false.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
    public static BadObject LessThan(BadObject left, BadObject right, BadSourcePosition pos)
	{
		if (left is IBadNumber lNum && right is IBadNumber rNum)
		{
			return lNum.Value < rNum.Value ? BadObject.True : BadObject.False;
		}

		throw new BadRuntimeException($"Can not apply operator '<' to {left} and {right}", pos);
	}

	public static IEnumerable<BadObject> LessThanWithOverride(
		BadExecutionContext context,
		BadObject left,
		BadObject right,
		BadSourcePosition position)
	{
		if (left.HasProperty(BadStaticKeys.LessOperatorName))
		{
			foreach (BadObject o in ExecuteOperatorOverride(left,
				         right,
				         context,
				         BadStaticKeys.LessOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else if (right.HasProperty(BadStaticKeys.LessOperatorName))
		{
			foreach (BadObject o in ExecuteOperatorOverride(right,
				         left,
				         context,
				         BadStaticKeys.LessOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else
		{
			yield return LessThan(left, right, position);
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

		foreach (BadObject o in LessThanWithOverride(context, left, right, Position))
		{
			yield return o;
		}
	}

	protected override string GetSymbol()
	{
		return "<";
	}
}
