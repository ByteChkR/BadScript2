using System.Text;

using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;
using BadScript2.Parser.Validation.Validators;
using BadScript2.Runtime.Error;

namespace BadScript2.Parser.Validation;

/// <summary>
///     Implements a context for expression validation.
/// </summary>
public readonly struct BadExpressionValidatorContext
{
    /// <summary>
    ///     Returns a string representation of the validation results.
    /// </summary>
    /// <returns>A string representation of the validation results.</returns>
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

    /// <summary>
    ///     The messages generated by the validators.
    /// </summary>
    private readonly List<BadExpressionValidatorMessage> m_Messages = new List<BadExpressionValidatorMessage>();

    /// <summary>
    ///     The messages generated by the validators.
    /// </summary>
    public IReadOnlyList<BadExpressionValidatorMessage> Messages => m_Messages;

    /// <summary>
    ///     Indicates whether there are any messages of type Error.
    /// </summary>
    public bool IsError => m_Messages.Any(m => m.Type == BadExpressionValidatorMessageType.Error);

    /// <summary>
    ///     The validators used by this context.
    /// </summary>
    private readonly List<BadExpressionValidator> m_Validators = new List<BadExpressionValidator>
    {
        new BadFunctionWithoutReturnTypeHasReturnWithExpressionValidator(),
        new BadFunctionWithReturnTypeHasReturnWithoutExpressionValidator(),
        new BadFunctionNameIsReservedKeywordValidator(),
        new BadFunctionParameterNameIsReservedKeywordValidator(),
        new BadFunctionReturnTypeIsNotNullButNotAllPathsHaveAReturnStatementValidator(),
        new BadDuplicateFunctionParameterNameValidator(),
        new BadConstantIfBranchValidator(),
        new BadEmptyForBlockValidator(),
        new BadEmptyForEachBlockValidator(),
        new BadEmptyIfBranchValidator(),
        new BadEmptyLockBlockValidator(),
        new BadEmptyUsingBlockValidator(),
        new BadEmptySwitchStatementValidator(),
    };

    /// <summary>
    ///     Adds an error message to the context.
    /// </summary>
    /// <param name="message">Error Text</param>
    /// <param name="parentExpression">The parent expression of the expression that caused the error.</param>
    /// <param name="expression">The expression that caused the error.</param>
    /// <param name="validator">The validator that caused the error.</param>
    public void AddError(
        string message,
        BadExpression parentExpression,
        BadExpression expression,
        BadExpressionValidator validator)
    {
        m_Messages.Add(
            new BadExpressionValidatorMessage(
                message,
                expression,
                validator,
                BadExpressionValidatorMessageType.Error,
                parentExpression
            )
        );
    }

    /// <summary>
    ///     Adds a warning message to the context.
    /// </summary>
    /// <param name="message">Warning Text</param>
    /// <param name="parentExpression">The parent expression of the expression that caused the warning.</param>
    /// <param name="expression">The expression that caused the warning.</param>
    /// <param name="validator">The validator that caused the warning.</param>
    public void AddWarning(
        string message,
        BadExpression parentExpression,
        BadExpression expression,
        BadExpressionValidator validator)
    {
        m_Messages.Add(
            new BadExpressionValidatorMessage(
                message,
                expression,
                validator,
                BadExpressionValidatorMessageType.Warning,
                parentExpression
            )
        );
    }

    /// <summary>
    ///     Adds an info message to the context.
    /// </summary>
    /// <param name="message">Info Text</param>
    /// <param name="parentExpression">The parent expression of the expression that caused the info.</param>
    /// <param name="expression">The expression that caused the info.</param>
    /// <param name="validator">The validator that caused the info.</param>
    public void AddInfo(
        string message,
        BadExpression parentExpression,
        BadExpression expression,
        BadExpressionValidator validator)
    {
        m_Messages.Add(
            new BadExpressionValidatorMessage(
                message,
                expression,
                validator,
                BadExpressionValidatorMessageType.Info,
                parentExpression
            )
        );
    }

    /// <summary>
    ///     Adds a validator to the context.
    /// </summary>
    /// <param name="validator">The validator to add.</param>
    public void AddValidator(BadExpressionValidator validator)
    {
        m_Validators.Add(validator);
    }

    /// <summary>
    ///     Creates a new context.
    /// </summary>
    public BadExpressionValidatorContext() { }

    /// <summary>
    ///     Validates the given expression.
    /// </summary>
    /// <param name="expression">The expression to validate.</param>
    /// <returns>The context.</returns>
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

    /// <summary>
    ///     Validates the given expressions.
    /// </summary>
    /// <param name="expressions">The expressions to validate.</param>
    /// <returns>The context.</returns>
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

    /// <summary>
    ///     Validates the given expressions.
    /// </summary>
    /// <param name="expressions">The expressions to validate.</param>
    /// <returns>The context.</returns>
    public static BadExpressionValidatorContext Validate(IEnumerable<BadExpression> expressions)
    {
        BadExpressionValidatorContext result = new BadExpressionValidatorContext().ValidateExpressions(expressions);

        return result;
    }

    /// <summary>
    ///     Validates the given expressions and throws an exception if there are any errors.
    /// </summary>
    /// <param name="expressions">The expressions to validate.</param>
    /// <exception cref="BadRuntimeException">If there are any errors.</exception>
    public static void ValidateOrThrow(IEnumerable<BadExpression> expressions)
    {
        BadExpressionValidatorContext result = Validate(expressions);

        if (result.IsError)
        {
            throw new BadRuntimeException(result.ToString());
        }
    }
}