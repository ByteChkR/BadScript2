namespace BadScript2.Parser.Validation;

/// <summary>
/// Defines the type of a <see cref="BadExpressionValidatorMessage" />.
/// </summary>
public enum BadExpressionValidatorMessageType
{
    /// <summary>
    /// Informational message.
    /// </summary>
    Info,
    /// <summary>
    /// Warning message.
    /// </summary>
    Warning,
    /// <summary>
    /// Error message.
    /// </summary>
    Error,
}