using BadScript2.Parser.Expressions.Block.Loop;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any expressions in the for block.
/// </summary>
public class BadEmptyForBlockValidator : BadExpressionValidator<BadForExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadForExpression expr)
    {
        if (!expr.Body.Any())
        {
            context.AddError(
                "For statement has no expressions",
                expr,
                expr,
                this
            );
        }
    }
}