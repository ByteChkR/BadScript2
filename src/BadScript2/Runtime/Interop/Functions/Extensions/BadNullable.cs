namespace BadScript2.Runtime.Interop.Functions.Extensions;

public readonly struct BadNullable<T>
{
    public readonly Type Type;
    public readonly T? Value;
    public readonly bool HasValue;
    public BadNullable()
    {
        Type = typeof(T);
        HasValue = false;
    }

    public BadNullable(T value)
    {
        Type = typeof(T);
        Value = value;
        HasValue = true;
    }
    
    public static implicit operator BadNullable<T>(T? value)
    {
        return new BadNullable<T>(value);
    }
    
    public static implicit operator T?(BadNullable<T> value)
    {
        return value.Value;
    }
}