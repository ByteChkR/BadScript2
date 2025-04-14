namespace BadScript2.Common;

/// <summary>
///     The base class of all BadScript Exceptions
/// </summary>
public abstract class BadScriptException : Exception
{
    /// <summary>
    /// Creates a new BadScriptException
    /// </summary>
    /// <param name="message">The Error Message</param>
    protected BadScriptException(string message) : base(message)
    {
        OriginalMessage = message;
    }

    /// <summary>
    /// Creates a new BadScriptException
    /// </summary>
    /// <param name="message">The Error Message</param>
    /// <param name="originalMessage">The Original Error Message</param>
    protected BadScriptException(string message, string originalMessage) : base(message)
    {
        OriginalMessage = originalMessage;
    }

    /// <summary>
    /// Creates a new BadScriptException
    /// </summary>
    /// <param name="message">The Error Message</param>
    /// <param name="originalMessage">The Original Error Message</param>
    /// <param name="inner">The Inner Exception</param>
    protected BadScriptException(string message, string originalMessage, Exception inner) : base(message, inner)
    {
        OriginalMessage = originalMessage;
    }

    /// <summary>
    /// Creates a new BadScriptException
    /// </summary>
    /// <param name="message">The Error Message</param>
    /// <param name="originalMessage">The Original Error Message</param>
    /// <param name="position">The Source Position</param>
    protected BadScriptException(string message, string originalMessage, BadSourcePosition position) : base(message)
    {
        OriginalMessage = originalMessage;
        Position = position;
    }

    /// <summary>
    /// Creates a new BadScriptException
    /// </summary>
    /// <param name="message">The Error Message</param>
    /// <param name="originalMessage">The Original Error Message</param>
    /// <param name="position">The Source Position</param>
    /// <param name="inner">The Inner Exception</param>
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