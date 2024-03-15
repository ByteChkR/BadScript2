using BadScript2.Parser;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     The Any Prototype, Base type for all types.
/// </summary>
public class BadAnyPrototype : BadClassPrototype
{
    /// <summary>
    ///     The Instance of the BadAnyPrototype
    /// </summary>
    public static readonly BadAnyPrototype Instance = new BadAnyPrototype();

    /// <summary>
    ///     Creates a new BadAnyPrototype
    /// </summary>
    public BadAnyPrototype() : base(
        "any",
        new BadMetaData(
            "This class represents the any type. It is the base class for all types and can be used to represent any type.",
            "Creates a new instance of the any type.",
            "any",
            new Dictionary<string, BadParameterMetaData>()
        )
    ) { }

    /// <inheritdoc />
    public override bool IsAbstract => true;
    protected override BadClassPrototype? BaseClass { get; }

    public override IReadOnlyCollection<BadInterfacePrototype> Interfaces { get; } = Array.Empty<BadInterfacePrototype>();

    /// <inheritdoc />
    public override bool IsAssignableFrom(BadObject obj)
    {
        return true;
    }

    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        throw new BadAbstractClassException(Name);
    }
}