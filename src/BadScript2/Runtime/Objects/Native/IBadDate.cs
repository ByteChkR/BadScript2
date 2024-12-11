namespace BadScript2.Runtime.Objects.Native;

public interface IBadDate : IBadNative
{
    /// <summary>
    ///     The Date Value
    /// </summary>
    new DateTimeOffset Value { get; }
}