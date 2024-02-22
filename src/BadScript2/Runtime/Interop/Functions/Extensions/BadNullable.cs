namespace BadScript2.Runtime.Interop.Functions.Extensions;

/// <summary>
///     This is a helper type that can be used when using the .SetFunction extensions to allow for nullable parameters.
/// </summary>
/// <typeparam name="T">The Type of the Parameter</typeparam>
public readonly struct BadNullable<T>
{
    /// <summary>
    ///     The Null Value
    /// </summary>
    public static readonly BadNullable<T> Null = new BadNullable<T>();

    /// <summary>
    ///     The Value of the Parameter
    /// </summary>
    public readonly T? Value;

    /// <summary>
    ///     The HasValue Flag
    /// </summary>
    public readonly bool HasValue;

    /// <summary>
    ///     Creates a new BadNullable
    /// </summary>
    public BadNullable()
    {
        HasValue = false;
        Value = default;
    }

    /// <summary>
    ///     Creates a new BadNullable
    /// </summary>
    /// <param name="value">The Value of the Parameter</param>
    public BadNullable(T value)
    {
        Value = value;
        HasValue = true;
    }

    /// <summary>
    ///     Converts a Nullable Type to a BadNullable
    /// </summary>
    /// <param name="value">The Nullable Value</param>
    /// <returns>A new BadNullable</returns>
    public static implicit operator BadNullable<T>(T? value)
    {
        return value == null ? Null : new BadNullable<T>(value);
    }

    /// <summary>
    ///     Converts a BadNullable to a Nullable Type
    /// </summary>
    /// <param name="value">The BadNullable</param>
    /// <returns>The Nullable Value</returns>
    public static implicit operator T?(BadNullable<T> value)
    {
        return value.Value;
    }
}