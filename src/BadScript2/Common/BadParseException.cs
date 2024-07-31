namespace BadScript2.Common;

/// <summary>
///     Base Class for all BadScript Parser Exceptions
/// </summary>
public abstract class BadParseException : BadScriptException
{
	/// <summary>
	///     Constructor for the BadParseException
	/// </summary>
	/// <param name="message">The Exception Message</param>
	/// <param name="position">The source position of where the error occurred</param>
	protected BadParseException(string message, BadSourcePosition position) : base(GetMessage(message, position),
	                                                                               message,
	                                                                               position
	                                                                              ) { }


	/// <summary>
	///     Constructor for the BadParseException
	/// </summary>
	/// <param name="message">The Exception Message</param>
	/// <param name="position">The source position of where the error occurred</param>
	/// <param name="inner">The Inner Exception</param>
	protected BadParseException(string message, BadSourcePosition position, Exception inner) :
        base(GetMessage(message, position),
             message,
             position,
             inner
            ) { }


	/// <summary>
	///     Returns a string that represents the error message with the source position
	/// </summary>
	/// <param name="message">The message</param>
	/// <param name="position">The source position</param>
	/// <returns>String representation</returns>
	private static string GetMessage(string message, BadSourcePosition position)
    {
        return $"{message} at {position.GetExcerpt(10, 10)} in {position.GetPositionInfo()}";
    }
}