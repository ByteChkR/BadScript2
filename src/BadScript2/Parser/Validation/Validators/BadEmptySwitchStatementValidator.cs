using BadScript2.Parser.Expressions.Block;
namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any expressions in the if branches block.
/// </summary>
public class BadEmptySwitchStatementValidator : BadExpressionValidator<BadSwitchExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadSwitchExpression expr)
    {
        if (expr.Cases.Count == 0 || expr.Cases.All(x => x.Value.Length == 0))
        {
            context.AddError(
                "Switch statement has no cases",
                expr,
                expr,
                this
            );
        }
    }
}