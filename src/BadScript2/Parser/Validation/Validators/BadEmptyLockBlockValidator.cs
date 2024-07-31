using BadScript2.Parser.Expressions.Block.Lock;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any expressions in the lock block.
/// </summary>
public class BadEmptyLockBlockValidator : BadExpressionValidator<BadLockExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadLockExpression expr)
    {
        if (!expr.Block.Any())
        {
            context.AddError("Lock statement has no expressions",
                             expr,
                             expr,
                             this
                            );
        }
    }
}