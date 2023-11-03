using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Runtime.Objects.Functions;

namespace BadScript2.Parser.Validation.Validators;

public class BadFunctionParameterNameIsReservedKeywordValidator : BadExpressionValidator<BadFunctionExpression>
{
	protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
	{
		foreach (BadFunctionParameter parameter in expr.Parameters)
		{
			if (BadStaticKeys.IsReservedKeyword(parameter.Name))
			{
				context.AddError($"Parameter name '{parameter.Name}' is a reserved keyword",
					expr,
					expr,
					this);
			}
		}
	}
}
