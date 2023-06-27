namespace BadScript2.SourceGenerators;

public abstract class BadAutoWrappedObject<T> : BadScript2.Runtime.Objects.Native.BadNative<T>
{
    protected BadAutoWrappedObject(T value) : base(value)
    {
    }

    public static implicit operator T(BadAutoWrappedObject<T> obj)
    {
        return obj.Value;
    }
}