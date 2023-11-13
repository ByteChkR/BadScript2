using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Binary.Math;

public class BadNegationExpression : BadExpression
{
	public BadNegationExpression(BadSourcePosition position, BadExpression expression) : base(
		expression.IsConstant,
		position)
	{
		Expression = expression;
	}

	public BadExpression Expression { get; }

	public override IEnumerable<BadExpression> GetDescendants()
	{
		return Expression.GetDescendantsAndSelf();
	}

	public static IEnumerable<BadObject> NegateWithOverride(
		BadExecutionContext? context,
		BadObject left,
		BadSourcePosition position)
	{
		if (left.HasProperty(BadStaticKeys.MultiplyOperatorName, context?.Scope))
		{
			foreach (BadObject o in ExecuteOperatorOverride(left,
				         left,
				         context!,
				         BadStaticKeys.MultiplyOperatorName,
				         position))
			{
				yield return o;
			}
		}
		else
		{
			yield return Negate(left, position);
		}
	}

	public static BadObject Negate(BadObject obj, BadSourcePosition pos)
	{
		if (obj is IBadNumber num)
		{
			return BadObject.Wrap(-num.Value);
		}

		throw new BadRuntimeException($"Can not apply operator '-' to {obj}", pos);
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject left = BadObject.Null;

		foreach (BadObject o in Expression.Execute(context))
		{
			left = o;

			yield return o;
		}

		left = left.Dereference();

		foreach (BadObject o in NegateWithOverride(context, left, Position))
		{
			yield return o;
		}
	}
}
