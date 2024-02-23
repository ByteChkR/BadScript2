using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains the BadScript2 Constant Folding Optimizations
/// </summary>
namespace BadScript2.Optimizations.Folding;

/// <summary>
///     Implements a simple constant folding optimization
/// </summary>
public static class BadConstantFoldingOptimizer
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
            BadLogger.Log($"Optimizing Expression: '{expr}' with Constant Folding", "Optimize");
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
        return exprs.Select(Optimize);
    }
}