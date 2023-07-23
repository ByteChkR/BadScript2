namespace BadScript2.Runtime.Objects.Types;

public static class BadInterfaceTools
{
	public static IEnumerable<BadInterfacePrototype> GetAllInterfaces(this BadClassPrototype interfce)
	{
		if (interfce is BadInterfacePrototype interfacePrototype)
		{
			yield return interfacePrototype;
		}

		BadClassPrototype? baseClass = interfce.GetBaseClass();

		if (baseClass != null)
		{
			foreach (BadInterfacePrototype i in baseClass.GetAllInterfaces())
			{
				yield return i;
			}
		}

		foreach (BadInterfacePrototype? i in interfce.Interfaces)
		{
			foreach (BadInterfacePrototype x in i.GetAllInterfaces())
			{
				yield return x;
			}
		}
	}

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
