using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
/// Checks if there are any expressions in the if branches block.
/// </summary>
public class BadEmptyIfBranchValidator : BadExpressionValidator<BadIfExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadIfExpression expr)
    {
        foreach (KeyValuePair<BadExpression,BadExpression[]> branch in expr.ConditionalBranches)
        {
            if(branch.Value.Length == 0)
            {
                context.AddError(
                    "If branch has no expressions",
                    expr,
                    branch.Key,
                    this
                );
            }
        }
        
        if (expr.ElseBranch == null)
        {
            return;
        }
        
        if(!expr.ElseBranch.Any())
        {
            context.AddError(
                "Else statement has no expressions",
                expr,
                expr,
                this
            );
        }
    }
}