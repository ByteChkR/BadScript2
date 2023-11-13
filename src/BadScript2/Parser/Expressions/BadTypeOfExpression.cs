using BadScript2.Common;
using BadScript2.Runtime;
using BadScript2.Runtime.Objects;

namespace BadScript2.Parser.Expressions;

/// <summary>
///     Implements the 'typeof' operator.
/// </summary>
public class BadTypeOfExpression : BadExpression
{
	/// <summary>
	///     The Expression to get the type of
	/// </summary>
	public readonly BadExpression Expression;

	/// <summary>
	///     Creates a new 'typeof' expression
	/// </summary>
	/// <param name="expression">Expression to get the type of</param>
	/// <param name="position">Source Position</param>
	public BadTypeOfExpression(BadExpression expression, BadSourcePosition position) : base(false, position)
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

		obj = obj.Dereference();

		yield return obj.GetPrototype();
	}
}
