using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Logic;

/// <summary>
///     Implements the Logic Exclusive Or Expression
/// </summary>
public class BadLogicXOrExpression : BadBinaryExpression
{
    /// <summary>
    ///     Constructor of the Logic Exclusive Or Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="position">Source Position of the Expression</param>
    public BadLogicXOrExpression(BadExpression left, BadExpression right, BadSourcePosition position) : base(left,
		right,
		position) { }

    /// <summary>
    ///     Returns true if left or right are true. False if both are true
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="right">Right side of the Expression</param>
    /// <param name="pos">Source position that is used to generate an Exception if left or right are not a boolean</param>
    /// <returns>True if either the left side or the right side are true. Otherwise false.</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the Left or Right side are not inheriting from IBadBoolean</exception>
    public static BadObject XOr(BadObject left, BadObject right, BadSourcePosition pos)
	{
		if (left is IBadBoolean lBool && right is IBadBoolean rBool)
		{
			return lBool.Value ^ rBool.Value ? BadObject.True : BadObject.False;
		}

		throw new BadRuntimeException($"Can not apply operator '^' to {left} and {right}", pos);
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

		yield return XOr(left, right, Position);
	}

	protected override string GetSymbol()
	{
		return "^";
	}
}
