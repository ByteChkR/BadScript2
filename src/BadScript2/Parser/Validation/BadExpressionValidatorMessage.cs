using BadScript2.Parser.Expressions;

namespace BadScript2.Parser.Validation;

/// <summary>
///     Models a message from a <see cref="BadExpressionValidator" />.
/// </summary>
public readonly struct BadExpressionValidatorMessage
{
    /// <summary>
    ///     Message Text.
    /// </summary>
    public readonly string Message;

    /// <summary>
    ///     The expression that caused the message.
    /// </summary>
    public readonly BadExpression Expression;

    /// <summary>
    ///     The parent expression of the expression that caused the message.
    /// </summary>
    public readonly BadExpression ParentExpression;

    /// <summary>
    ///     The validator that generated the message.
    /// </summary>
    public readonly BadExpressionValidator Validator;

    /// <summary>
    ///     The type of the message.
    /// </summary>
    public readonly BadExpressionValidatorMessageType Type;

    /// <summary>
    ///     Creates a new message.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="expression">The expression that caused the message.</param>
    /// <param name="validator">The validator that generated the message.</param>
    /// <param name="type">The type of the message.</param>
    /// <param name="parentExpression">The parent expression of the expression that caused the message.</param>
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