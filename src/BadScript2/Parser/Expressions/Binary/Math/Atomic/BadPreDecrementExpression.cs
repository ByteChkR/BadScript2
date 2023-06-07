using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Pre Decrement Expression
/// </summary>
public class BadPreDecrementExpression : BadExpression
{
    /// <summary>
    ///     Right side of the Expression
    /// </summary>
    public readonly BadExpression Right;

    public override IEnumerable<BadExpression> GetDescendants()
    {

	    foreach (BadExpression? expression in Right.GetDescendantsAndSelf())
	    {
		    yield return expression;
	    }
    }
    /// <summary>
    ///     Constructor of the Pre Decrement Expression
    /// </summary>
    /// <param name="right">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPreDecrementExpression(BadExpression right, BadSourcePosition position) : base(right.IsConstant,
		position)
	{
		Right = right;
	}

	public static BadObject Decrement(BadObjectReference reference, BadSourcePosition position)
	{
		BadObject right = reference.Dereference();

		if (right is not IBadNumber leftNumber)
		{
			throw new BadRuntimeException("Right side of -- must be a number", position);
		}

		BadObject r = leftNumber.Value - 1;
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
			throw new BadRuntimeException("Right side of -- must be a reference", Position);
		}

		yield return Decrement(rightRef, Position);
	}
}
