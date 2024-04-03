using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if the function has a return type but the return statement does not have an expression.
/// </summary>
public class
    BadFunctionWithReturnTypeHasReturnWithoutExpressionValidator : BadReturnExpressionValidator
{
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        if (expr.TypeExpression is null or BadVariableExpression { Name: "void" })
        {
            return;
        }

        foreach (BadReturnExpression? retExpr in GetReturnExpressions(expr.Body))
        {
            if (retExpr.Right == null)
            {
                context.AddError(
                    "The function has a return type but the return statement does not have an expression.",
                    expr,
                    retExpr,
                    this
                );
            }
        }
    }
}