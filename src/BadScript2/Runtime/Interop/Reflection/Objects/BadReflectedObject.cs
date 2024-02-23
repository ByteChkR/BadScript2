using BadScript2.Runtime.Objects;
using BadScript2.Runtime.Objects.Native;
using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Interop.Reflection.Objects;

/// <summary>
///     Implements a Reflected Object
/// </summary>
public class BadReflectedObject : BadObject
{
    /// <summary>
    ///     The Prototype for the Reflected Object Type
    /// </summary>
    public static readonly BadClassPrototype Prototype = new BadReflectedObjectPrototype();

    /// <summary>
    ///     Creates a new Reflected Object
    /// </summary>
    /// <param name="instance"></param>
    public BadReflectedObject(object instance)
    {
        Instance = instance;
        Type = instance.GetType();
        Members = BadReflectedMemberTable.Create(Type);
    }

    /// <summary>
    ///     The Type of the Reflected Object
    /// </summary>
    public Type Type { get; }

    /// <summary>
    ///     The Instance of the Reflected Object
    /// </summary>
    public object Instance { get; }

    /// <summary>
    ///     The Member Table for the Reflected Object
    /// </summary>
    public BadReflectedMemberTable Members { get; }


    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return Instance.ToString();
    }

    /// <inheritdoc />
    public override BadObjectReference GetProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is IBadString s && Members.Contains(s.Value))
        {
            return Members.GetMember(Instance, s.Value);
        }

        return base.GetProperty(propName, caller);
    }

    /// <inheritdoc />
    public override bool HasProperty(BadObject propName, BadScope? caller = null)
    {
        if (propName is IBadString s)
        {
            return Members.Contains(s.Value) || base.HasProperty(propName, caller);
        }

        return base.HasProperty(propName, caller);
    }
}