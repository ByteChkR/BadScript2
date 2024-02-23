using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if there are any constant expressions in the if branches conditions.
/// </summary>
public class BadConstantIfBranchValidator : BadExpressionValidator<BadIfExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadIfExpression expr)
    {
        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expr.ConditionalBranches)
        {
            if (branch.Key.IsConstant)
            {
                context.AddError(
                    "If branch condition is constant",
                    expr,
                    branch.Key,
                    this
                );
            }
        }
    }
}