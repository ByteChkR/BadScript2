using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected Member
/// </summary>
public abstract class BadReflectedMember
{
    /// <summary>
    ///     Creates a new BadReflectedMember
    /// </summary>
    /// <param name="name">The Name of the Member</param>
    protected BadReflectedMember(string name)
    {
        Name = name;
    }

    /// <summary>
    ///     Indicates if the Member is ReadOnly
    /// </summary>
    public abstract bool IsReadOnly { get; }

    /// <summary>
    ///     The Name of the Member
    /// </summary>
    protected string Name { get; }

    /// <summary>
    ///     Gets the Member from an Instance
    /// </summary>
    /// <param name="instance">The Instance to get the Member from</param>
    /// <returns>The Member</returns>
    public abstract BadObject Get(object instance);

    /// <summary>
    ///     Sets the Member on an Instance
    /// </summary>
    /// <param name="instance">The Instance to set the Member on</param>
    /// <param name="o">The Value to set</param>
    public abstract void Set(object instance, BadObject o);

    /// <summary>
    ///     Wraps an Object into a BadObject
    /// </summary>
    /// <param name="o">The Object to wrap</param>
    /// <returns>The Wrapped Object</returns>
    protected static BadObject Wrap(object? o)
    {
        return BadObject.CanWrap(o) ? BadObject.Wrap(o) : new BadReflectedObject(o!);
    }
}