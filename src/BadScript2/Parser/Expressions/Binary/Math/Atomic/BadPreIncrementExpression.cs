using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Pre Increment Expression
/// </summary>
public class BadPreIncrementExpression : BadExpression
{
    /// <summary>
    ///     Right side of the Expression
    /// </summary>
    public readonly BadExpression Right;

    /// <summary>
    ///     Constructor of the Pre Increment Expression
    /// </summary>
    /// <param name="right">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPreIncrementExpression(BadExpression right, BadSourcePosition position) : base(right.IsConstant,
		position)
	{
		Right = right;
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
		{
			yield return expression;
		}
	}

	public static BadObject Increment(BadObjectReference reference, BadSourcePosition position)
	{
		BadObject right = reference.Dereference();

		if (right is not IBadNumber leftNumber)
		{
			throw new BadRuntimeException("Right side of ++ must be a number", position);
		}

		BadObject r = leftNumber.Value + 1;
		reference.Set(r);

		return r;
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject right = BadObject.Null;

		foreach (BadObject o in Right.Execute(context))
		{
			right = o;

			yield return o;
		}

		if (right is not BadObjectReference rightRef)
		{
			throw new BadRuntimeException("Right side of ++ must be a reference", Position);
		}

		foreach (BadObject o in IncrementWithOverride(context, rightRef, Position))
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

		if (left.HasProperty(BadStaticKeys.PreIncrementOperatorName, context?.Scope))
		{
			foreach (BadObject o in ExecuteOperatorOverride(left,
				         context!,
				         BadStaticKeys.PreIncrementOperatorName,
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
