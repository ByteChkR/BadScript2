using BadScript2.Common;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

public class BadFunctionNameIsReservedKeywordValidator : BadExpressionValidator<BadFunctionExpression>
{
	protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
	{
		if (BadStaticKeys.IsReservedKeyword(expr.Name?.ToString() ?? string.Empty))
		{
			context.AddError($"Name {expr.Name} is a reserved keyword",
				expr,
				expr,
				this);
		}
	}
}
