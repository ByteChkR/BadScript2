using BadScript2.Runtime.Error;
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
    private readonly BadClassPrototype? m_Prototype;

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


    /// <summary>
    ///     Creates a new Function Constraint
    /// </summary>
    /// <param name="name">Name of the Function</param>
    /// <param name="return">Return Type of the Function</param>
    /// <param name="returnProto">Return Type Prototype</param>
    /// <param name="parameters">The Function Parameters</param>
    public BadInterfaceFunctionConstraint(string name, BadExpression? @return, BadClassPrototype returnProto, BadFunctionParameter[] parameters)
    {
        Name = name;
        Return = @return;
        Parameters = parameters;
        m_Prototype = returnProto;
    }


    public override string ToString()
    {
        return "FunctionConstraint";
    }

    public override void Validate(BadClass obj, List<BadInterfaceValidatorError> errors)
    {
        if (Return != null && m_Prototype == null)
        {
            throw new BadRuntimeException("Return Type was not Evaluated.");
        }

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

        if (m_Prototype != null && !m_Prototype.IsSuperClassOf(f.ReturnType))
        {
            errors.Add(
                new BadInterfaceValidatorError(
                    $"Return Type Mismatch. Expected {m_Prototype} but got {f.ReturnType} in {f}",
                    this
                )
            );
        }

        if (f.Parameters.Length != Parameters.Length)
        {
            errors.Add(
                new BadInterfaceValidatorError(
                    $"Parameter Count Mismatch. Expected {Parameters.Length} but got {f.Parameters.Length} in {f}",
                    this
                )
            );

            return;
        }


        for (int i = 0; i < Parameters.Length; i++)
        {
            BadFunctionParameter actual = f.Parameters[i];
            BadFunctionParameter expected = Parameters[i];

            if (actual.IsOptional != expected.IsOptional)
            {
                errors.Add(
                    new BadInterfaceValidatorError(
                        $"{f}: Parameter Optional Flags are not equal. Implementation: {actual}, Expectation: {expected}",
                        this
                    )
                );

                return;
            }

            if (actual.IsNullChecked != expected.IsNullChecked)
            {
                errors.Add(
                    new BadInterfaceValidatorError(
                        $"{f}: Parameter Null Check Flags are not equal. Implementation: {actual}, Expectation: {expected}",
                        this
                    )
                );

                return;
            }

            if (actual.IsRestArgs != expected.IsRestArgs)
            {
                errors.Add(
                    new BadInterfaceValidatorError(
                        $"{f}: Parameter Rest Args Flags are not equal. Implementation: {actual}, Expectation: {expected}",
                        this
                    )
                );


                return;
            }

            if (actual.TypeExpr != null)
            {
                if (expected.Type != null && actual.Type != expected.Type)
                {
                    errors.Add(
                        new BadInterfaceValidatorError(
                            $"{f}: Parameter Types not equal. Implementation: {actual}, Expectation: {expected}",
                            this
                        )
                    );

                    return;
                }
            }
        }
    }
}