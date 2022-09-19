using BadScript2.Common;

namespace BadScript2.Parser;

/// <summary>
/// Gets Raised when a Parser Error occurs
/// </summary>
public class BadParserException : BadParseException
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The Exception Message</param>
    /// <param name="position">The Source Position of the Exception</param>
    public BadParserException(
        string message,
        BadSourcePosition position) : base(message, position) { }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The Exception Message</param>
    /// <param name="position">The Source Position of the Exception</param>
    /// <param name="inner">The Inner Exception</param>
    public BadParserException(
        string message,
        BadSourcePosition position,
        Exception inner) :
        base(message, position, inner) { }
}