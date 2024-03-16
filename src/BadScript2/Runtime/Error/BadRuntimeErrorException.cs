using BadScript2.Common;

namespace BadScript2.Runtime.Error;

/// <summary>
///     Gets thrown if the runtime encounters an error
/// </summary>
public class BadRuntimeErrorException : BadScriptException
{
	public BadRuntimeError Error { get; }

	/// <summary>
	///     Creates a new BadRuntimeErrorException
	/// </summary>
	/// <param name="error">The Runtime Error that was generated</param>
	public BadRuntimeErrorException(BadRuntimeError error) : base(
		error.ToString(),
		error.ErrorObject.ToString()
	)
	{
		Error = error;
	}
}