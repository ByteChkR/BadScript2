using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Validation;

public readonly struct BadExpressionValidatorMessage
{
	public readonly string Message;
	public readonly BadExpression Expression;
	public readonly BadExpression ParentExpression;
	public readonly BadExpressionValidator Validator;
	public readonly BadExpressionValidatorMessageType Type;

	public BadExpressionValidatorMessage(
		string message,
		BadExpression expression,
		BadExpressionValidator validator,
		BadExpressionValidatorMessageType type,
		BadExpression parentExpression)
	{
		Message = message;
		Expression = expression;
		Validator = validator;
		Type = type;
		ParentExpression = parentExpression;
	}
}
