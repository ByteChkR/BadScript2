using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Parser.Validation.Validators;

public class
	BadFunctionWithoutReturnTypeHasReturnWithExpressionValidator : BadReturnExpressionValidator
{
	protected override void Validate(BadExpressionValidatorContext context, BadFunctionExpression expr)
	{
		if (expr.TypeExpression == null)
		{
			foreach (BadReturnExpression e in GetReturnExpressions(expr.Body))
			{
				if (e.Right != null)
				{
					context.AddWarning($"The Return statement '{e}' can not return a value.",
						expr,
						e,
						this);
				}
			}
		}
	}
}
