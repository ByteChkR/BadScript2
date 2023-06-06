using BadScript2.Common;
using BadScript2.Runtime.Settings;

namespace BadScript2.Runtime.Error;

/// <summary>
///     Gets thrown by the runtime
/// </summary>
public class BadRuntimeException : BadScriptException
{
    /// <summary>
    ///     Creates a new BadScriptException
    /// </summary>
    /// <param name="message">Exception Message</param>
    public BadRuntimeException(string message) : base(message) { }


    /// <summary>
    ///     Creates a new BadScriptException
    /// </summary>
    /// <param name="message">Exception Message</param>
    /// <param name="position">The Source Position</param>
    public BadRuntimeException(string message, BadSourcePosition position) : base(
		$"{message} at {position.GetPositionInfo()}",
		message,
		position) { }

	public static BadRuntimeException Create(BadScope? scope, string message)
	{
		if (scope != null && BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors)
		{
			message = message + Environment.NewLine + scope.GetStackTrace();
		}

		return new BadRuntimeException(message);
	}

	public static BadRuntimeException Create(BadScope? scope, string message, BadSourcePosition pos)
	{
		if (scope != null && BadRuntimeSettings.Instance.WriteStackTraceInRuntimeErrors)
		{
			message = message + Environment.NewLine + scope.GetStackTrace();
		}

		return new BadRuntimeException(message, pos);
	}
}
