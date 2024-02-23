using BadScript2.Parser.Expressions.Block;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any expressions in the using block.
/// </summary>
public class BadEmptyUsingBlockValidator : BadExpressionValidator<BadUsingExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadUsingExpression expr)
    {
        if (!expr.Expressions.Any())
        {
            context.AddError(
                "Using statement has no expressions",
                expr,
                expr,
                this
            );
        }
    }
}