using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Comparison;

/// <summary>
///     Implements the Greater or Equal Expression
///     <Left> >= <Right>
/// </summary>
public class BadGreaterOrEqualExpression : BadBinaryExpression
{
	/// <summary>
	///     Constructor of the Greater or Equal Expression
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="position">Source Position of the Expression</param>
	public BadGreaterOrEqualExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

	/// <summary>
	///     Returns true if the left side is greater or equal to the right side
	/// </summary>
	/// <param name="left">Left side of the Expression</param>
	/// <param name="right">Right side of the Expression</param>
	/// <param name="pos">Source position that is used to generate an Exception if left or right are not a number</param>
	/// <returns>True if the Left side is greater or equal than the right side. Otherwise false.</returns>
	/// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadNumber</exception>
	public static BadObject GreaterOrEqual(BadObject left, BadObject right, BadSourcePosition pos)
	{
		if (left is IBadNumber lNum && right is IBadNumber rNum)
		{
			return lNum.Value >= rNum.Value ? BadObject.True : BadObject.False;
		}

		throw new BadRuntimeException($"Can not apply operator '>=' to {left} and {right}", pos);
	}

	public static IEnumerable<BadObject> GreaterOrEqualWithOverride(
		BadExecutionContext context,
		BadObject left,
		BadObject right,
		BadSourcePosition position)
	{
		if (left.HasProperty(BadStaticKeys.GreaterEqualOperatorName))
		{
			foreach (BadObject o in ExecuteOperatorOverride(left,
				         right,
				         context,
				         BadStaticKeys.GreaterEqualOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else if (right.HasProperty(BadStaticKeys.GreaterEqualOperatorName))
		{
			foreach (BadObject o in ExecuteOperatorOverride(right,
				         left,
				         context,
				         BadStaticKeys.GreaterEqualOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else
		{
			yield return GreaterOrEqual(left, right, position);
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

		foreach (BadObject o in GreaterOrEqualWithOverride(context, left, right, Position))
		{
			yield return o;
		}
	}

	protected override string GetSymbol()
	{
		return ">=";
	}
}
