using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Interop.Reflection.Objects.Members;

public abstract class BadReflectedMember
{
    protected BadReflectedMember(string name)
    {
        Name = name;
    }

    protected string Name { get; }

    public abstract BadObject Get(object instance);

    protected BadObject Wrap(object? o)
    {
        if (BadObject.CanWrap(o))
        {
            return BadObject.Wrap(o);
        }

        return new BadReflectedObject(o!);
    }
}