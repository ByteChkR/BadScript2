using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks if the function parameter name is a reserved keyword.
/// </summary>
public class BadFunctionParameterNameIsReservedKeywordValidator : BadExpressionValidator<BadFunctionExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        foreach (BadFunctionParameter parameter in expr.Parameters)
        {
            if (BadStaticKeys.IsReservedKeyword(parameter.Name))
            {
                context.AddError($"Parameter name '{parameter.Name}' is a reserved keyword",
                                 expr,
                                 expr,
                                 this
                                );
            }
        }
    }
}