using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Expressions.Variables;
namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if the function has no return type but returns a value.
/// </summary>
public class
    BadFunctionWithoutReturnTypeHasReturnWithExpressionValidator : BadReturnExpressionValidator
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        if (expr.TypeExpression != null && expr.TypeExpression is not BadVariableExpression { Name: "void" })
        {
            return;
        }

        if (expr.IsSingleLine)
        {
            context.AddInfo(
                "The Function is declared as a single line function, but has no return type. The Runtime will implicitly wrap the expression body of the function into a return expression. This can lead to unexpected behaviour when the expression returns a value.",
                expr,
                expr.Body.First(),
                this
            );
        }
        else
        {
            foreach (BadReturnExpression e in GetReturnExpressions(expr.Body))
            {
                if (e.Right != null)
                {
                    context.AddWarning(
                        $"The Return statement '{e}' can not return a value.",
                        expr,
                        e,
                        this
                    );
                }
            }
        }
    }
}