using BadScript2.Parser;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects.Types.Interface;
namespace BadScript2.Runtime.Objects.Types;

/// <summary>
///     The Void Prototype, can be assigned to nothing, can not be inherited from, can not be instantiated.
///     Denotes an absence of a return value.
/// </summary>
public class BadVoidPrototype : BadClassPrototype
{
    /// <summary>
    ///     Creates a new BadVoidPrototype
    /// </summary>
    private BadVoidPrototype() : base("void", BadMetaData.Empty) { }
    /// <summary>
    ///     The Instance of the BadVoidPrototype
    /// </summary>
    public static BadVoidPrototype Instance { get; } = new BadVoidPrototype();

    public static BadObject Object { get; } = new BadVoidObject();

    /// <inheritdoc />
    protected override BadClassPrototype? BaseClass { get; } = null;
    /// <inheritdoc />
    public override bool IsAbstract { get; } = true;
    /// <inheritdoc />
    public override IReadOnlyCollection<BadInterfacePrototype> Interfaces { get; } = Array.Empty<BadInterfacePrototype>();
    /// <inheritdoc />
    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        throw BadRuntimeException.Create(caller.Scope, "Cannot create an instance of void");
    }

    /// <inheritdoc />
    public override bool IsAssignableFrom(BadObject obj)
    {
        return obj == Object;
    }

    /// <inheritdoc />
    public override bool IsSuperClassOf(BadClassPrototype proto)
    {
        return proto == Instance;
    }

    private class BadVoidObject : BadObject
    {
        public override BadClassPrototype GetPrototype()
        {
            return Instance;
        }

        public override string ToSafeString(List<BadObject> done)
        {
            return "<void>";
        }

        public override string ToString()
        {
            return "<void>";
        }

        public override bool HasProperty(string propName, BadScope? caller = null)
        {
            return false;
        }

        public override BadObjectReference GetProperty(string propName, BadScope? caller = null)
        {
            throw BadRuntimeException.Create(caller, $"Property '{propName}' does not exist on void");
        }
    }
}