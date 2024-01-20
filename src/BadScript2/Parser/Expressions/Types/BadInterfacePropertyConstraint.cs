using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Parser.Expressions.Types;

public class BadInterfacePropertyConstraint : BadInterfaceConstraint
{
    /// <summary>
    /// The Property Prototype
    /// </summary>
    private readonly BadClassPrototype? m_Prototype;
    /// <summary>
    /// The Property Name
    /// </summary>
    public readonly string Name;
    
    /// <summary>
    /// The Property Type Expression
    /// </summary>
    public readonly BadExpression? Type;

    /// <summary>
    /// Creates a new Property Constraint
    /// </summary>
    /// <param name="name">The Property Name</param>
    /// <param name="type">The Property Type Expression</param>
    public BadInterfacePropertyConstraint(string name, BadExpression? type)
    {
        Name = name;
        Type = type;
    }

    /// <summary>
    /// Creates a new Property Constraint
    /// </summary>
    /// <param name="name">The Property Name</param>
    /// <param name="type">The Property Type Expression</param>
    /// <param name="prototype">The Property Prototype</param>
    public BadInterfacePropertyConstraint(string name, BadExpression? type, BadClassPrototype? prototype)
    {
        Name = name;
        Type = type;
        m_Prototype = prototype;
    }

    /// <inheritdoc cref="BadInterfaceConstraint.Validate" />
    public override void Validate(BadClass obj, List<BadInterfaceValidatorError> errors)
    {
        if (Type != null && m_Prototype == null)
        {
            throw new BadRuntimeException("Type was not Evaluated.");
        }

        if (!obj.HasProperty(Name, obj.Scope))
        {
            errors.Add(new BadInterfaceValidatorError($"Missing Property. Expected {Name}", this));

            return;
        }

        BadPropertyInfo info = obj.Scope.GetVariableInfo(Name);
        BadClassPrototype actual = info.Type ?? BadAnyPrototype.Instance;
        BadClassPrototype expected = m_Prototype ?? BadAnyPrototype.Instance;
        if (!expected.IsSuperClassOf(actual))
        {
            errors.Add(new BadInterfaceValidatorError($"Property Type Mismatch. Expected {expected} but got {actual}", this));
        }
    }
}