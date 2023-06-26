using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions;

public class BadDeleteExpression : BadExpression
{
	public readonly BadExpression Expression;

	public BadDeleteExpression(BadExpression expression, BadSourcePosition position) : base(false, position)
	{
		Expression = expression;
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression e in Expression.GetDescendantsAndSelf())
		{
			yield return e;
		}
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject? obj = BadObject.Null;

		foreach (BadObject o in Expression.Execute(context))
		{
			obj = o;

			yield return o;
		}

		if (context.Scope.IsError)
		{
			yield break;
		}

		if (obj is not BadObjectReference r)
		{
			throw BadRuntimeException.Create(context.Scope, $"Cannot delete {Expression}", Position);
		}

		r.Delete();

		yield return BadObject.Null;
	}
}