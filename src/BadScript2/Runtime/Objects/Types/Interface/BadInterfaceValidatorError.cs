namespace BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
///     Contains data about a validation error
/// </summary>
public readonly struct BadInterfaceValidatorError
{
	/// <summary>
	///     Creates a new Error
	/// </summary>
	/// <param name="message">The Error Message</param>
	/// <param name="constraint">The Constraint that caused the error</param>
	public BadInterfaceValidatorError(string message, BadInterfaceConstraint constraint)
	{
		Message = message;
		Constraint = constraint;
	}

	/// <summary>
	///     The Error Message
	/// </summary>
	public string Message { get; }

	/// <summary>
	///     The Constraint that caused the error
	/// </summary>
	public BadInterfaceConstraint Constraint { get; }

	public override string ToString()
	{
		return $"-- {Constraint} | {Message}";
	}
}
