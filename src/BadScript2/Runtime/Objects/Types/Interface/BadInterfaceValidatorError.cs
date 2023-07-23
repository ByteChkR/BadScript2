namespace BadScript2.Runtime.Objects.Types;

public readonly struct BadInterfaceValidatorError
{
	public BadInterfaceValidatorError(string message, BadInterfaceConstraint constraint)
	{
		Message = message;
		Constraint = constraint;
	}

	public string Message { get; }

	public BadInterfaceConstraint Constraint { get; }

	public override string ToString()
	{
		return $"-- {Constraint} | {Message}";
	}
}
