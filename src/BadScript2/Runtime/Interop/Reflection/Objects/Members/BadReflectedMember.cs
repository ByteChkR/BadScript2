using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

/// <summary>
///     Implements a Reflected Member
/// </summary>
public abstract class BadReflectedMember
{
    protected BadReflectedMember(string name)
    {
        Name = name;
    }

    public abstract bool IsReadOnly { get; }

    protected string Name { get; }

    public abstract BadObject Get(object instance);

    public abstract void Set(object instance, BadObject o);

    protected static BadObject Wrap(object? o)
    {
        return BadObject.CanWrap(o) ? BadObject.Wrap(o) : new BadReflectedObject(o!);
    }
}