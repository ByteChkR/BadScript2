namespace BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
///     Implements an Interface Constraint
/// </summary>
public abstract class BadInterfaceConstraint
{
	/// <summary>
	///     Validates the given Object against this Constraint
	/// </summary>
	/// <param name="obj">The Object to validate</param>
	/// <param name="errors">The Error List to add errors to</param>
	public abstract void Validate(BadClass obj, List<BadInterfaceValidatorError> errors);
}
