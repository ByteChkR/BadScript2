using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Post Increment Expression
/// </summary>
public class BadPostIncrementExpression : BadExpression
{
    /// <summary>
    ///     Left side of the expression
    /// </summary>
    public readonly BadExpression Left;

    /// <summary>
    ///     Constructor of the Post Increment Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPostIncrementExpression(BadExpression left, BadSourcePosition position) : base(left.IsConstant,
		position)
	{
		Left = left;
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression? expression in Left.GetDescendantsAndSelf())
		{
			yield return expression;
		}
	}

	public static BadObject Increment(BadObjectReference reference, BadSourcePosition position)
	{
		BadObject value = reference.Dereference();

		if (value is not IBadNumber leftNumber)
		{
			throw new BadRuntimeException("Left side of ++ must be a number", position);
		}

		reference.Set(leftNumber.Value + 1);

		return value;
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject left = BadObject.Null;

		foreach (BadObject o in Left.Execute(context))
		{
			left = o;

			yield return o;
		}

		if (left is not BadObjectReference leftRef)
		{
			throw new BadRuntimeException("Left side of ++ must be a reference", Position);
		}

		foreach (BadObject o in IncrementWithOverride(context, leftRef, Position))
		{
			yield return o;
		}
	}

	public static IEnumerable<BadObject> IncrementWithOverride(
		BadExecutionContext? context,
		BadObjectReference leftRef,
		BadSourcePosition position)
	{
		BadObject left = leftRef.Dereference();

		if (left.HasProperty(BadStaticKeys.PostIncrementOperatorName, context?.Scope))
		{
			foreach (BadObject o in ExecuteOperatorOverride(left,
				         context!,
				         BadStaticKeys.PostIncrementOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else
		{
			yield return Increment(leftRef, position);
		}
	}
}
