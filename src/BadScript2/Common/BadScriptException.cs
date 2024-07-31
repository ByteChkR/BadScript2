namespace BadScript2.Common;

/// <summary>
///     The base class of all BadScript Exceptions
/// </summary>
public abstract class BadScriptException : Exception
{
    protected BadScriptException(string message) : base(message)
    {
        OriginalMessage = message;
    }

    protected BadScriptException(string message, string originalMessage) : base(message)
    {
        OriginalMessage = originalMessage;
    }

    protected BadScriptException(string message, string originalMessage, Exception inner) : base(message, inner)
    {
        OriginalMessage = originalMessage;
    }

    protected BadScriptException(string message, string originalMessage, BadSourcePosition position) : base(message)
    {
        OriginalMessage = originalMessage;
        Position = position;
    }

    protected BadScriptException(string message,
                                 string originalMessage,
                                 BadSourcePosition position,
                                 Exception inner) : base(message, inner)
    {
        OriginalMessage = originalMessage;
        Position = position;
    }

    /// <summary>
    ///     The Original Error Message
    /// </summary>
    public string OriginalMessage { get; }

    /// <summary>
    ///     The source position of where the error occurred
    /// </summary>
    public BadSourcePosition? Position { get; }
}