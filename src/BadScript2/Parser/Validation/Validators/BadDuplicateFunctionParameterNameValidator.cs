using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects.Functions;

/// <summary>
/// Contains the Expression Validators for the BadScript2 Language
/// </summary>
namespace BadScript2.Parser.Validation.Validators;

/// <summary>
///     Checks for duplicate parameter names in function definitions.
/// </summary>
public class BadDuplicateFunctionParameterNameValidator : BadExpressionValidator<BadFunctionExpression>
{
    /// <inheritdoc cref="BadExpressionValidator{T}.Validate" />
    protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
    {
        HashSet<string> names = new HashSet<string>();

        foreach (BadFunctionParameter parameter in expr.Parameters)
        {
            if (!names.Add(parameter.Name))
            {
                context.AddError(
                    $"Duplicate parameter name '{parameter.Name}'",
                    expr,
                    expr,
                    this
                );
            }
        }
    }
}