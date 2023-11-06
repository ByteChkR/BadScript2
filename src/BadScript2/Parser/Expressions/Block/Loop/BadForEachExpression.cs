using BadScript2.Common;
using BadScript2.Optimizations.Folding;
using BadScript2.Reader.Token;
using BadScript2.Runtime;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Native;

namespace BadScript2.Parser.Expressions.Block.Loop;

/// <summary>
///     Implements the For Each Expression
/// </summary>
public class BadForEachExpression : BadExpression
{
    /// <summary>
    ///     The Variable Name of the Current Loop iteration
    /// </summary>
    public readonly BadWordToken LoopVariable;

    /// <summary>
    ///     The Loop Body
    /// </summary>
    private readonly List<BadExpression> m_Body;

    /// <summary>
    ///     The Enumerable/Enumerator Expression of the Loop
    /// </summary>
    public BadExpression Target;

    /// <summary>
    ///     Constructor of the For Each Expression
    /// </summary>
    /// <param name="target">The Enumerable/Enumerator Expression of the Loop</param>
    /// <param name="loopVariable">The Variable Name of the Current Loop iteration</param>
    /// <param name="body">The Loop Body</param>
    /// <param name="position">The Source Position of the Expression</param>
    public BadForEachExpression(
		BadExpression target,
		BadWordToken loopVariable,
		BadExpression[] body,
		BadSourcePosition position) : base(false,
		position)
	{
		Target = target;
		LoopVariable = loopVariable;
		m_Body = body.ToList();
	}

	public IEnumerable<BadExpression> Body => m_Body;

	public void SetBody(IEnumerable<BadExpression> body)
	{
		m_Body.Clear();
		m_Body.AddRange(body);
	}

	public override void Optimize()
	{
		Target = BadConstantFoldingOptimizer.Optimize(Target);

		for (int i = 0; i < m_Body.Count; i++)
		{
			m_Body[i] = BadConstantFoldingOptimizer.Optimize(m_Body[i]);
		}
	}

	public override IEnumerable<BadExpression> GetDescendants()
	{
		foreach (BadExpression expression in Target.GetDescendantsAndSelf())
		{
			yield return expression;
		}

		foreach (BadExpression expression in m_Body)
		{
			foreach (BadExpression descendant in expression.GetDescendantsAndSelf())
			{
				yield return descendant;
			}
		}
	}

    /// <summary>
    ///     Helper Function that returns the MoveNext/GetCurrent function of the Target
    /// </summary>
    /// <param name="target">The loop target</param>
    /// <returns>Tuple of Functions used in the for each loop</returns>
    /// <exception cref="BadRuntimeException">Gets thrown if the target is not an enumerable object.</exception>
    private (BadFunction moveNext, BadFunction getCurrent) FindEnumerator(BadObject target, BadExecutionContext context)
	{
		if (target.HasProperty("MoveNext"))
		{
			BadFunction? moveNext = target.GetProperty("MoveNext", context.Scope).Dereference() as BadFunction;

			if (moveNext != null)
			{
				if (target.HasProperty("GetCurrent"))
				{
					BadFunction? getCurrent =
						target.GetProperty("GetCurrent", context.Scope).Dereference() as BadFunction;

					if (getCurrent != null)
					{
						return (moveNext, getCurrent);
					}
				}
			}
		}

		throw new BadRuntimeException("Invalid enumerator", Position);
	}

	protected override IEnumerable<BadObject> InnerExecute(BadExecutionContext context)
	{
		BadObject target = BadObject.Null;

		foreach (BadObject o in Target.Execute(context))
		{
			target = o;

			yield return o;
		}

		if (context.Scope.IsError)
		{
			yield break;
		}

		target = target.Dereference();

		if (target.HasProperty("GetEnumerator"))
		{
			BadFunction? getEnumerator =
				target.GetProperty("GetEnumerator", context.Scope).Dereference() as BadFunction;

			if (getEnumerator == null)
			{
				throw new BadRuntimeException("Invalid enumerator", Position);
			}

			BadObject newTarget = BadObject.Null;

			foreach (BadObject o in getEnumerator.Invoke(Array.Empty<BadObject>(), context))
			{
				yield return o;

				newTarget = o;
			}

			if (context.Scope.IsError)
			{
				yield break;
			}

			if (newTarget == BadObject.Null)
			{
				throw BadRuntimeException.Create(context.Scope, $"Invalid enumerator: {target}", Position);
			}

			target = newTarget.Dereference();
		}

		(BadFunction moveNext, BadFunction getCurrent) = FindEnumerator(target, context);

		if (context.Scope.IsError)
		{
			yield break;
		}

		BadObject cond = BadObject.Null;

		foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), context))
		{
			cond = o;

			yield return o;
		}

		if (context.Scope.IsError)
		{
			yield break;
		}

		IBadBoolean bRet = cond.Dereference() as IBadBoolean ??
		                   throw new BadRuntimeException("While Condition is not a boolean", Position);

		while (bRet.Value)
		{
			BadExecutionContext loopContext = new BadExecutionContext(context.Scope.CreateChild("ForEachLoop",
				context.Scope,
				null,
				BadScopeFlags.Breakable | BadScopeFlags.Continuable));

			BadObject current = BadObject.Null;

			foreach (BadObject o in getCurrent.Invoke(Array.Empty<BadObject>(), loopContext))
			{
				current = o;

				yield return o;
			}

			if (loopContext.Scope.IsError)
			{
				yield break;
			}

			current = current.Dereference();

			loopContext.Scope.DefineVariable(LoopVariable.Text, current);

			foreach (BadObject o in loopContext.Execute(m_Body))
			{
				yield return o;
			}

			if (loopContext.Scope.IsBreak || loopContext.Scope.ReturnValue != null || loopContext.Scope.IsError)
			{
				break;
			}

			foreach (BadObject o in moveNext.Invoke(Array.Empty<BadObject>(), loopContext))
			{
				cond = o;

				yield return o;
			}


			if (loopContext.Scope.IsError)
			{
				yield break;
			}

			bRet = cond.Dereference() as IBadBoolean ??
			       throw BadRuntimeException.Create(context.Scope,
				       "Enumerator MoveNext did not return a boolean",
				       Position);
		}
	}
}
