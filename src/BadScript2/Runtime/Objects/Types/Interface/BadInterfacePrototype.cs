using BadScript2.Parser;
using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Objects.Types.Interface;

/// <summary>
///     Implements a BadScript Interface Prototype
/// </summary>
public class BadInterfacePrototype : BadClassPrototype
{
    /// <summary>
    ///     The Prototype for the Interface Prototype
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = new BadNativeClassPrototype<BadClassPrototype>(
        "Interface",
        (_, _) => throw new BadRuntimeException("Interfaces cannot be extended")
    );

    /// <summary>
    ///     Backing Function of the Constraints Property
    /// </summary>
    private readonly Func<BadInterfaceConstraint[]> m_ConstraintsFunc;

    /// <summary>
    ///     The Constraints of this Interface
    /// </summary>
    private BadInterfaceConstraint[]? m_Constraints;

    /// <summary>
    ///     Creates a new Interface Prototype
    /// </summary>
    /// <param name="name">The Name of the Interface</param>
    /// <param name="interfaces">The Interfaces this Interface extends</param>
    /// <param name="metaData">The Meta Data of the Interface</param>
    /// <param name="constraints">The Constraints of the Interface</param>
    public BadInterfacePrototype(
        string name,
        BadInterfacePrototype[] interfaces,
        BadMetaData? metaData,
        Func<BadInterfaceConstraint[]> constraints) : base(
        name,
        null,
        interfaces,
        metaData
    )
    {
        m_ConstraintsFunc = constraints;
    }


    /// <inheritdoc />
    public override bool IsAbstract => true;

    /// <summary>
    ///     The Constraints of this Interface
    /// </summary>
    public IEnumerable<BadInterfaceConstraint> Constraints => m_Constraints ??= m_ConstraintsFunc();

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        throw BadRuntimeException.Create(caller.Scope, "Interfaces cannot be instantiated");
    }

    /// <inheritdoc />
    public override bool IsSuperClassOf(BadClassPrototype proto)
    {
        return base.IsSuperClassOf(proto) || proto.Interfaces.Any(IsSuperClassOf);
    }


    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return $"interface {Name}";
    }
}