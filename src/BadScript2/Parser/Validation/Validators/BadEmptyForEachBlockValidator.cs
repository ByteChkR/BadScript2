using BadScript2.Parser.Expressions.Block.Loop;
namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any expressions in the foreach block.
/// </summary>
public class BadEmptyForEachBlockValidator : BadExpressionValidator<BadForEachExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadForEachExpression expr)
    {
        if (!expr.Body.Any())
        {
            context.AddError(
                "For-Each statement has no expressions",
                expr,
                expr,
                this
            );
        }
    }
}