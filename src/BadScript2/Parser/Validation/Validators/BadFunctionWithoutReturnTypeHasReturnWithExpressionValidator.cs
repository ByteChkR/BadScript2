using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
/// Checks if the function has no return type but returns a value.
/// </summary>
public class
    BadFunctionWithoutReturnTypeHasReturnWithExpressionValidator : BadReturnExpressionValidator
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        if (expr.TypeExpression != null)
        {
            return;
        }

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