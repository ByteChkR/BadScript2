using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Optimizations;

/// <summary>
///     Implements a simple constant folding optimization
/// </summary>
public static class BadExpressionOptimizer
{
    /// <summary>
    ///     Optimizes the given expression
    /// </summary>
    /// <param name="expr">The expression to optimize</param>
    /// <returns>Optimized Expression</returns>
    public static BadExpression Optimize(BadExpression expr)
	{
		if (expr is not IBadNativeExpression && expr.IsConstant)
		{
			BadLogger.Log($"Optimizing Expression: '{expr}'", "Runtime");
			BadObject obj = expr.Execute(null!).Last();

			return new BadConstantExpression(expr.Position, obj);
		}

		expr.Optimize();

		return expr;
	}

    /// <summary>
    ///     Optimizes a list of expressions
    /// </summary>
    /// <param name="exprs">Expression</param>
    /// <returns>Optimized Expressions</returns>
    public static IEnumerable<BadExpression> Optimize(IEnumerable<BadExpression> exprs)
	{
		foreach (BadExpression expression in exprs)
		{
			yield return Optimize(expression);
		}
	}
}
