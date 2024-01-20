namespace BadScript2.Runtime.Interop.Functions.Extensions;

public readonly struct BadNullable<T>
{
    public readonly T? Value;
    public readonly bool HasValue;

    public BadNullable()
    {
        HasValue = false;
        Value = default;
    }

    public BadNullable(T value)
    {
        Value = value;
        HasValue = true;
    }

    public static implicit operator BadNullable<T>(T? value)
    {
        if (value == null)
        {
            return new BadNullable<T>();
        }

        return new BadNullable<T>(value);
    }

    public static implicit operator T?(BadNullable<T> value)
    {
        return value.Value;
    }
}