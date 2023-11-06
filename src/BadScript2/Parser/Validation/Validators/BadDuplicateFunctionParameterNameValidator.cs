using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Validation.Validators;

public class BadDuplicateFunctionParameterNameValidator : BadExpressionValidator<BadFunctionExpression>
{
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