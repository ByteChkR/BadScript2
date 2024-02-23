using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if the function name is a reserved keyword.
/// </summary>
public class BadFunctionNameIsReservedKeywordValidator : BadExpressionValidator<BadFunctionExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        if (BadStaticKeys.IsReservedKeyword(expr.Name?.ToString() ?? string.Empty))
        {
            context.AddError(
                $"Name {expr.Name} is a reserved keyword",
                expr,
                expr,
                this
            );
        }
    }
}