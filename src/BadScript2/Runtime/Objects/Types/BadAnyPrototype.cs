using BadScript2.Parser;
using BadScript2.Runtime.Objects.Types.Interface;

namespace BadScript2.Runtime.Objects.Types;

public class BadAnyPrototype : BadClassPrototype
{
    public static readonly BadAnyPrototype Instance = new BadAnyPrototype();

    public BadAnyPrototype() : base(
        "any",
        null,
        Array.Empty<BadInterfacePrototype>(),
        new BadMetaData(
            "This class represents the any type. It is the base class for all types and can be used to represent any type.",
            "Creates a new instance of the any type.",
            "any",
            new Dictionary<string, BadParameterMetaData>()
        )
    ) { }

    public override bool IsAbstract => true;

    public override bool IsAssignableFrom(BadObject obj)
    {
        return true;
    }

    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        throw new BadAbstractClassException(Name);
    }
}