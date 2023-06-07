using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math.Atomic;

/// <summary>
///     Implements the Post Decrement Expression
/// </summary>
public class BadPostDecrementExpression : BadExpression
{
    /// <summary>
    ///     Left side of the expression
    /// </summary>
    public readonly BadExpression Left;

    public override IEnumerable<BadExpression> GetDescendants()
    {

	    foreach (BadExpression? expression in Left.GetDescendantsAndSelf())
	    {
		    yield return expression;
	    }
    }
    /// <summary>
    ///     Constructor of the Post Decrement Expression
    /// </summary>
    /// <param name="left">Left side of the Expression</param>
    /// <param name="position">Source position of the Expression</param>
    public BadPostDecrementExpression(BadExpression left, BadSourcePosition position) : base(left.IsConstant,
		position)
	{
		Left = left;
	}


	public static BadObject Decrement(BadObjectReference reference, BadSourcePosition position)
	{
		BadObject value = reference.Dereference();

		if (value is not IBadNumber leftNumber)
		{
			throw new BadRuntimeException("Left side of -- must be a number", position);
		}

		reference.Set(leftNumber.Value - 1);

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
			throw new BadRuntimeException("Left side of -- must be a reference", Position);
		}


		yield return Decrement(leftRef, Position);
	}
}
