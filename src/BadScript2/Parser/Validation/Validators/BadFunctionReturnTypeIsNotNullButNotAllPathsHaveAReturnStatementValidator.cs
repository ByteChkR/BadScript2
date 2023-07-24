using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

public class
	BadFunctionReturnTypeIsNotNullButNotAllPathsHaveAReturnStatementValidator : BadExpressionValidator<
		BadFunctionExpression>
{
	private void Validate(BadExpressionPath parent, BadExpression expr)
	{
		if (expr is BadIfExpression ifExpr)
		{
			Validate(parent, ifExpr);
		}
		else if (expr is BadWhileExpression whileExpr)
		{
			Validate(parent, whileExpr);
		}
		else if (expr is BadForExpression forExpr)
		{
			Validate(parent, forExpr);
		}
		else if (expr is BadForEachExpression forEachExpr)
		{
			Validate(parent, forEachExpr);
		}
		else if (expr is BadLockExpression lockExpr)
		{
			Validate(parent, lockExpr);
		}
		else if (expr is BadTryCatchExpression tryCatchExpr)
		{
			Validate(parent, tryCatchExpr);
		}
		else if (expr is BadReturnExpression)
		{
			parent.SetHasReturnStatement();
		}
	}

	private void Validate(BadExpressionPath parent, BadWhileExpression expr)
	{
		BadExpressionPath path = new BadExpressionPath(expr);
		parent.AddChildPath(path);

		foreach (BadExpression expression in expr.Body)
		{
			Validate(path, expression);
		}
	}

	private void Validate(BadExpressionPath parent, BadForExpression expr)
	{
		BadExpressionPath path = new BadExpressionPath(expr);
		parent.AddChildPath(path);

		foreach (BadExpression expression in expr.Body)
		{
			Validate(path, expression);
		}
	}

	private void Validate(BadExpressionPath parent, BadForEachExpression expr)
	{
		BadExpressionPath path = new BadExpressionPath(expr);
		parent.AddChildPath(path);

		foreach (BadExpression expression in expr.Body)
		{
			Validate(path, expression);
		}
	}

	private void Validate(BadExpressionPath parent, BadLockExpression expr)
	{
		BadExpressionPath path = new BadExpressionPath(expr);
		parent.AddChildPath(path);

		foreach (BadExpression expression in expr.Block)
		{
			Validate(path, expression);
		}
	}

	private void Validate(BadExpressionPath parent, BadIfExpression expr)
	{
		BadExpressionPath ifParent = new BadExpressionPath(expr);
		parent.AddChildPath(ifParent);
		
		List<BadExpressionPath> paths = new List<BadExpressionPath>();

		foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expr.ConditionalBranches)
		{
			BadExpressionPath path = new BadExpressionPath(expr);
			ifParent.AddChildPath(path);
			paths.Add(path);
			foreach (BadExpression expression in branch.Value)
			{
				Validate(path, expression);
			}
		}

		BadExpressionPath elsePath = new BadExpressionPath(expr);
		ifParent.AddChildPath(elsePath);

		if (expr.ElseBranch != null)
		{
			foreach (BadExpression expression in expr.ElseBranch)
			{
				Validate(elsePath, expression);
			}
		}

	}

	private void Validate(BadExpressionPath parent, BadTryCatchExpression expr)
	{
		BadExpressionPath tryPath = new BadExpressionPath(expr);
		parent.AddChildPath(tryPath);

		foreach (BadExpression expression in expr.TryExpressions)
		{
			Validate(tryPath, expression);
		}

		BadExpressionPath catchPath = new BadExpressionPath(expr);
		parent.AddChildPath(catchPath);

		foreach (BadExpression expression in expr.CatchExpressions)
		{
			Validate(catchPath, expression);
		}
	}

	protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
	{
		BadExpressionPath path = new BadExpressionPath(expr);

		if (expr.TypeExpression != null)
		{
			//1. Go Through every expression inside the body.
			foreach (BadExpression e in expr.Body)
			{
				Validate(path, e);
			}

			if (!path.IsValid)
			{
				//find all invalid paths
				foreach (BadExpressionPath invalidPath in path.GetInvalidPaths())
				{
					context.AddError("The function has a return type but not all paths have a return statement.",
						expr,
						invalidPath.Parent,
						this);
				}
			}
		}
	}
}
