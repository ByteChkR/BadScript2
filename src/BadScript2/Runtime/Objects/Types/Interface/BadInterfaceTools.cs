namespace BadScript2.Runtime.Objects.Types;

/// <summary>
/// Extension Methods for BadInterface
/// </summary>
public static class BadInterfaceTools
{
	/// <summary>
	/// Returns all Interfaces implemented by this Type
	/// </summary>
	/// <param name="type"></param>
	/// <returns>Enumeration of Interface Prototypes</returns>
	public static IEnumerable<BadInterfacePrototype> GetAllInterfaces(this BadClassPrototype type)
	{
		if (type is BadInterfacePrototype interfacePrototype)
		{
			yield return interfacePrototype;
		}

		BadClassPrototype? baseClass = type.GetBaseClass();

		if (baseClass != null)
		{
			foreach (BadInterfacePrototype i in baseClass.GetAllInterfaces())
			{
				yield return i;
			}
		}

		foreach (BadInterfacePrototype? i in type.Interfaces)
		{
			foreach (BadInterfacePrototype x in i.GetAllInterfaces())
			{
				yield return x;
			}
		}
	}

	/// <summary>
	/// Validates a given Object against a set of Interfaces
	/// </summary>
	/// <param name="instance">The Object to validate</param>
	/// <param name="interfaces">The Interfaces to validate against</param>
	/// <returns>Validation Result</returns>
	public static BadInterfaceValidatorResult Validate(
		this BadClass instance,
		IEnumerable<BadInterfacePrototype> interfaces)
	{
		List<BadInterfaceValidatorError> errors = new List<BadInterfaceValidatorError>();

		BadInterfaceConstraint[] allConstraints = interfaces
			.SelectMany(x => x.GetAllInterfaces())
			.Distinct()
			.SelectMany(x => x.Constraints)
			.ToArray();

		foreach (BadInterfaceConstraint constraint in allConstraints)
		{
			constraint.Validate(instance, errors);
		}

		return new BadInterfaceValidatorResult(errors.ToArray());
	}
}
