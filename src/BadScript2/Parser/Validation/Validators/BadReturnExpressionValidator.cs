using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
/// Base class for Validators that work with return statements.
/// </summary>
public abstract class BadReturnExpressionValidator : BadExpressionValidator<BadFunctionExpression>
{
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
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

        if (expr.ElseBranch == null)
        {
            yield break;
        }

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
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadWhileExpression expr)
    {
        return expr.Body.SelectMany(GetReturnExpressions);
    }
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadUsingExpression expr)
    {
        return expr.Expressions.SelectMany(GetReturnExpressions);
    }
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadForExpression expr)
    {
        return expr.Body.SelectMany(GetReturnExpressions);
    }
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadForEachExpression expr)
    {
        return expr.Body.SelectMany(GetReturnExpressions);
    }
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadLockExpression expr)
    {
        return expr.Block.SelectMany(GetReturnExpressions);
    }
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
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
    /// <summary>
    /// Returns all return expressions in the given expression.
    /// </summary>
    /// <param name="expr">The expression to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(BadExpression expr)
    {
        switch (expr)
        {
            case BadIfExpression ifExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(ifExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadWhileExpression whileExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(whileExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadUsingExpression usingExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(usingExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadForExpression forExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(forExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadForEachExpression forEachExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(forEachExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadLockExpression lockExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(lockExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadTryCatchExpression tryCatchExpr:
            {
                foreach (BadReturnExpression? e in GetReturnExpressions(tryCatchExpr))
                {
                    yield return e;
                }

                break;
            }
            case BadReturnExpression retExpr:
                yield return retExpr;

                break;
        }
    }
    /// <summary>
    /// Returns all return expressions in the given expressions.
    /// </summary>
    /// <param name="expressions">The expressions to check.</param>
    /// <returns>All return expressions in the given expression.</returns>
    protected IEnumerable<BadReturnExpression> GetReturnExpressions(IEnumerable<BadExpression> expressions)
    {
        return expressions.SelectMany(GetReturnExpressions);
    }
}