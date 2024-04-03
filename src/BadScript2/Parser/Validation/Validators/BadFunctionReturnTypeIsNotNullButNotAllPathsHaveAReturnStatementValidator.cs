using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if the function has a return type but not all paths have a return statement.
/// </summary>
public class
    BadFunctionReturnTypeIsNotNullButNotAllPathsHaveAReturnStatementValidator : BadExpressionValidator<
    BadFunctionExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    private void Validate(BadExpressionPath parent, BadExpression expr)
    {
        switch (expr)
        {
            case BadIfExpression ifExpr:
                Validate(parent, ifExpr);

                break;
            case BadSwitchExpression switchExpr:
                Validate(parent, switchExpr);

                break;
            case BadWhileExpression whileExpr:
                Validate(parent, whileExpr);

                break;
            case BadForExpression forExpr:
                Validate(parent, forExpr);

                break;
            case BadForEachExpression forEachExpr:
                Validate(parent, forEachExpr);

                break;
            case BadLockExpression lockExpr:
                Validate(parent, lockExpr);

                break;
            case BadTryCatchExpression tryCatchExpr:
                Validate(parent, tryCatchExpr);

                break;
            case BadReturnExpression or BadThrowExpression:
                parent.SetHasReturnStatement();

                break;
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


        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expr.ConditionalBranches)
        {
            BadExpressionPath path = new BadExpressionPath(expr);
            ifParent.AddChildPath(path);

            foreach (BadExpression expression in branch.Value)
            {
                Validate(path, expression);
            }
        }

        BadExpressionPath elsePath = new BadExpressionPath(expr);
        ifParent.AddChildPath(elsePath);

        if (expr.ElseBranch == null)
        {
            return;
        }

        {
            foreach (BadExpression expression in expr.ElseBranch)
            {
                Validate(elsePath, expression);
            }
        }
    }
    
    

    private void Validate(BadExpressionPath parent, BadSwitchExpression expr)
    {
        BadExpressionPath switchParent = new BadExpressionPath(expr);
        parent.AddChildPath(switchParent);


        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expr.Cases)
        {
            BadExpressionPath path = new BadExpressionPath(expr);
            switchParent.AddChildPath(path);

            foreach (BadExpression expression in branch.Value)
            {
                Validate(path, expression);
            }
        }

        BadExpressionPath defaultPath = new BadExpressionPath(expr);
        switchParent.AddChildPath(defaultPath);

        if (expr.DefaultCase == null)
        {
            return;
        }

        {
            foreach (BadExpression expression in expr.DefaultCase)
            {
                Validate(defaultPath, expression);
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

        if (expr.TypeExpression is null or BadVariableExpression { Name: "void" })
        {
            return;
        }

        //1. Go Through every expression inside the body.
        foreach (BadExpression e in expr.Body)
        {
            Validate(path, e);
        }

        if (path.IsValid)
        {
            return;
        }

        //find all invalid paths
        foreach (BadExpressionPath invalidPath in path.GetInvalidPaths())
        {
            context.AddError(
                "The function has a return type but not all paths have a return statement.",
                expr,
                invalidPath.Parent,
                this
            );
        }
    }
}