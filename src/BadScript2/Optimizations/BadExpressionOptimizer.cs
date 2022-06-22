using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Optimizations;

public static class BadExpressionOptimizer
{
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

    public static IEnumerable<BadExpression> Optimize(IEnumerable<BadExpression> exprs)
    {
        foreach (BadExpression expression in exprs)
        {
            yield return Optimize(expression);
        }
    }
}