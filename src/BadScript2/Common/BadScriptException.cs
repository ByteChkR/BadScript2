namespace BadScript2.Common;

public abstract class BadScriptException : Exception
{
    /// <summary>
    ///     The Original Error Message
    /// </summary>
    public readonly string OriginalMessage;

    /// <summary>
    ///     The source position of where the error occurred
    /// </summary>
    public readonly BadSourcePosition? Position;

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

    protected BadScriptException(string message, string originalMessage, BadSourcePosition position, Exception inner) : base(message, inner)
    {
        OriginalMessage = originalMessage;
        Position = position;
    }
}