using System.Text;

using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Validation.Validators;

namespace BadScript2.Parser.Validation;

public readonly struct BadExpressionValidatorContext
{
	public override string ToString()
	{
		StringBuilder sb = new StringBuilder();

		if (IsError)
		{
			sb.AppendLine("Validation failed:");
		}
		else if (Messages.Count > 0)
		{
			sb.AppendLine("Validation succeeded with warnings:");
		}
		else
		{
			sb.AppendLine("Validation succeeded.");
		}

		foreach (BadExpressionValidatorMessage message in Messages)
		{
			
			sb.Append($"\t{message.Type}: {message.Message} in ");

			if (message.ParentExpression is BadFunctionExpression func)
			{
				sb.AppendLine($"'{func.GetHeader()}':{message.Expression.Position.GetPositionInfo()}");
			}
			else
			{
				sb.AppendLine($"{message.Expression.Position.GetPositionInfo()}");
			}
		}


		return sb.ToString();
	}

	private readonly List<BadExpressionValidatorMessage> m_Messages = new List<BadExpressionValidatorMessage>();

	public IReadOnlyList<BadExpressionValidatorMessage> Messages => m_Messages;

	public bool IsError => m_Messages.Any(m => m.Type == BadExpressionValidatorMessageType.Error);

	private readonly List<BadExpressionValidator> m_Validators = new List<BadExpressionValidator>
	{
		new BadFunctionWithoutReturnTypeHasReturnWithExpressionValidator(),
		new BadFunctionWithReturnTypeHasReturnWithoutExpressionValidator(),
		new BadFunctionNameIsReservedKeywordValidator(),
		new BadFunctionParameterNameIsReservedKeywordValidator(),
		new BadFunctionReturnTypeIsNotNullButNotAllPathsHaveAReturnStatementValidator(),
		new BadDuplicateFunctionParameterNameValidator()
	};

	public void AddError(
		string message,
		BadExpression parentExpression,
		BadExpression expression,
		BadExpressionValidator validator)
	{
		m_Messages.Add(new BadExpressionValidatorMessage(message,
			expression,
			validator,
			BadExpressionValidatorMessageType.Error,
			parentExpression));
	}

	public void AddWarning(
		string message,
		BadExpression parentExpression,
		BadExpression expression,
		BadExpressionValidator validator)
	{
		m_Messages.Add(new BadExpressionValidatorMessage(message,
			expression,
			validator,
			BadExpressionValidatorMessageType.Warning,
			parentExpression));
	}

	public void AddInfo(
		string message,
		BadExpression parentExpression,
		BadExpression expression,
		BadExpressionValidator validator)
	{
		m_Messages.Add(new BadExpressionValidatorMessage(message,
			expression,
			validator,
			BadExpressionValidatorMessageType.Info,
			parentExpression));
	}

	public void AddValidator(BadExpressionValidator validator)
	{
		m_Validators.Add(validator);
	}

	public BadExpressionValidatorContext() { }

	public BadExpressionValidatorContext ValidateExpressions(BadExpression expression)
	{
		foreach (BadExpression expr in expression.GetDescendantsAndSelf())
		{
			foreach (BadExpressionValidator? validator in m_Validators)
			{
				validator.Validate(this, expr);
			}
		}

		return this;
	}

	public BadExpressionValidatorContext ValidateExpressions(IEnumerable<BadExpression> expressions)
	{
		BadExpressionValidatorContext ctx = new BadExpressionValidatorContext();

		foreach (BadExpression expression in expressions)
		{
			foreach (BadExpression expr in expression.GetDescendantsAndSelf())
			{
				foreach (BadExpressionValidator? validator in m_Validators)
				{
					validator.Validate(ctx, expr);
				}
			}
		}

		return ctx;
	}

	public static BadExpressionValidatorContext Validate(IEnumerable<BadExpression> expressions)
	{
		BadExpressionValidatorContext result = new BadExpressionValidatorContext().ValidateExpressions(expressions);

		return result;
	}

	public static void ValidateOrThrow(IEnumerable<BadExpression> expressions)
	{
		BadExpressionValidatorContext result = Validate(expressions);

		if (result.IsError)
		{
			throw new Exception(result.ToString());
		}
	}
}
