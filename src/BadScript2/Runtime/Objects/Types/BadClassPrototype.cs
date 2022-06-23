using BadScript2.Runtime.Error;

namespace BadScript2.Runtime.Objects.Types;

public abstract class BadClassPrototype : BadObject
{
    public static readonly BadClassPrototype Prototype = new BadNativeClassPrototype<BadClassPrototype>(
        "Class",
        (_, _) => throw new BadRuntimeException("Classes cannot be extended")
    );

    protected readonly BadClassPrototype? BaseClass;
    public readonly string Name;

    protected BadClassPrototype(string name, BadClassPrototype? baseClass)
    {
        Name = name;
        BaseClass = baseClass;
    }

    public override BadClassPrototype GetPrototype()
    {
        return Prototype;
    }

    public abstract IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true);

    public bool IsSuperClassOf(BadClassPrototype proto)
    {
        return proto == this || (BaseClass?.IsSuperClassOf(proto) ?? false);
    }

    public virtual bool IsAssignableFrom(BadObject obj)
    {
        if (obj == Null)
        {
            return true;
        }

        if (obj is BadClass cls)
        {
            return cls.InheritsFrom(this);
        }

        return false;
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return $"prototype {Name}";
    }
}