using BadScript2.Common;

namespace BadScript2.Reader;

/// <summary>
/// Gets Raised if the Reader encounters an Error
/// </summary>
public class BadSourceReaderException : BadParseException
{
    /// <summary>
    /// Creates a new BadSourceReaderException
    /// </summary>
    /// <param name="message">The Exception Message</param>
    /// <param name="position">The Source Position of the Error</param>
    public BadSourceReaderException(string message, BadSourcePosition position) : base(message, position) { }

    
    /// <summary>
    /// Creates a new BadSourceReaderException
    /// </summary>
    /// <param name="message">The Exception Message</param>
    /// <param name="position">The Source Position of the Error</param>
    /// <param name="inner">The Inner Exception</param>
    public BadSourceReaderException(string message, BadSourcePosition position, Exception inner) : base(
        message,
        position,
        inner
    ) { }
}