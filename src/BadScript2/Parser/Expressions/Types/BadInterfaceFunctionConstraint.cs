using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Functions;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Parser.Expressions.Types;

/// <summary>
///     Implements a Function Constraint for an Interface
///     The Constraints specifies how a specific function should look like.
/// </summary>
public class BadInterfaceFunctionConstraint : BadInterfaceConstraint
{
	/// <summary>
	///     The Name of the Function
	/// </summary>
	public readonly string Name;

	/// <summary>
	///     The Parameters of the Function
	/// </summary>
	public readonly BadFunctionParameter[] Parameters;

	/// <summary>
	///     Return Type of the Function
	/// </summary>
	public readonly BadExpression? Return;

	/// <summary>
	///     Creates a new Function Constraint
	/// </summary>
	/// <param name="name">Name of the Function</param>
	/// <param name="return">Return Type of the Function</param>
	/// <param name="parameters">The Function Parameters</param>
	public BadInterfaceFunctionConstraint(string name, BadExpression? @return, BadFunctionParameter[] parameters)
	{
		Name = name;
		Return = @return;
		Parameters = parameters;
	}


	public override string ToString()
	{
		return "FunctionConstraint";
	}

	public override void Validate(BadClass obj, List<BadInterfaceValidatorError> errors)
	{
		if (!obj.HasProperty(Name, obj.Scope))
		{
			errors.Add(new BadInterfaceValidatorError($"Missing Property. Expected {Name}", this));

			return;
		}

		BadObject o = obj.GetProperty(Name).Dereference();

		if (o is not BadFunction f)
		{
			errors.Add(new BadInterfaceValidatorError($"Property {Name} is not a function", this));

			return;
		}

		if (f.Parameters.Length != Parameters.Length)
		{
			errors.Add(new BadInterfaceValidatorError(
				$"Parameter Count Mismatch. Expected {Parameters.Length} but got {f.Parameters.Length} in {f}",
				this));

			return;
		}

		for (int i = 0; i < Parameters.Length; i++)
		{
			BadFunctionParameter p = f.Parameters[i];
			BadFunctionParameter p2 = Parameters[i];

			if (p.IsOptional != p2.IsOptional)
			{
				errors.Add(new BadInterfaceValidatorError(
					$"{f}: Parameter Optional Flags are not equal. Implementation: {p}, Expectation: {p2}",
					this));

				return;
			}

			if (p.IsNullChecked != p2.IsNullChecked)
			{
				errors.Add(new BadInterfaceValidatorError(
					$"{f}: Parameter Null Check Flags are not equal. Implementation: {p}, Expectation: {p2}",
					this));

				return;
			}

			if (p.IsRestArgs != p2.IsRestArgs)
			{
				errors.Add(new BadInterfaceValidatorError(
					$"{f}: Parameter Rest Args Flags are not equal. Implementation: {p}, Expectation: {p2}",
					this));


				return;
			}

			if (p.TypeExpr != null)
			{
				if (p2.Type != null && p.Type != p2.Type)
				{
					errors.Add(new BadInterfaceValidatorError(
						$"{f}: Parameter Types not equal. Implementation: {p}, Expectation: {p2}",
						this));

					return;
				}
			}
		}
	}
}
