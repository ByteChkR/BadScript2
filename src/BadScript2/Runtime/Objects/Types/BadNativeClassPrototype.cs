namespace BadScript2.Runtime.Objects.Types;

public abstract class BadNativeClassPrototype : BadClassPrototype
{
    private readonly Func<BadExecutionContext, BadObject[], BadObject> m_Func;

    protected BadNativeClassPrototype(
        string name,
        Func<BadExecutionContext, BadObject[], BadObject> func) : base(name, null)
    {
        m_Func = func;
    }

    public IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, BadObject[] args)
    {
        yield return m_Func(caller, args);
    }

    public override IEnumerable<BadObject> CreateInstance(BadExecutionContext caller, bool setThis = true)
    {
        return CreateInstance(caller, Array.Empty<BadObject>());
    }
}

public class BadNativeClassPrototype<T> : BadNativeClassPrototype
    where T : BadObject
{
    public BadNativeClassPrototype(
        string name,
        Func<BadExecutionContext, BadObject[], BadObject> func) : base(name, func) { }

    public override bool IsAssignableFrom(BadObject obj)
    {
        if (obj == Null)
        {
            return true;
        }

        if (obj is T)
        {
            return true;
        }

        return false;
    }
}