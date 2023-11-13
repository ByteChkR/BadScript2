using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

public abstract class BadReturnExpressionValidator : BadExpressionValidator<BadFunctionExpression>
{
	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadIfExpression expr)
	{
		foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expr.ConditionalBranches)
		{
			foreach (BadExpression e in branch.Value)
			{
				foreach (BadReturnExpression r in GetReturnExpressions(e))
				{
					yield return r;
				}
			}
		}

		if (expr.ElseBranch != null)
		{
			foreach (BadExpression e in expr.ElseBranch)
			{
				foreach (BadReturnExpression r in GetReturnExpressions(e))
				{
					yield return r;
				}
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadWhileExpression expr)
	{
		foreach (BadExpression e in expr.Body)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadForExpression expr)
	{
		foreach (BadExpression e in expr.Body)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadForEachExpression expr)
	{
		foreach (BadExpression e in expr.Body)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadLockExpression expr)
	{
		foreach (BadExpression e in expr.Block)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadTryCatchExpression expr)
	{
		foreach (BadExpression e in expr.TryExpressions)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}

		foreach (BadExpression e in expr.CatchExpressions)
		{
			foreach (BadReturnExpression r in GetReturnExpressions(e))
			{
				yield return r;
			}
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadExpression expr)
	{
		if (expr is BadIfExpression ifExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(ifExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadWhileExpression whileExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(whileExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadForExpression forExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(forExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadForEachExpression forEachExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(forEachExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadLockExpression lockExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(lockExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadTryCatchExpression tryCatchExpr)
		{
			foreach (BadReturnExpression? e in GetReturnExpressions(tryCatchExpr))
			{
				yield return e;
			}
		}
		else if (expr is BadReturnExpression retExpr)
		{
			yield return retExpr;
		}
	}

	protected IEnumerable<BadReturnExpression> GetReturnExpressions(IEnumerable<BadExpression> expressions)
	{
		foreach (BadExpression expr in expressions)
		{
			foreach (BadReturnExpression e in GetReturnExpressions(expr))
			{
				yield return e;
			}
		}
	}
}
