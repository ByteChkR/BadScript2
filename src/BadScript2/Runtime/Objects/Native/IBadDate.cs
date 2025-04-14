namespace BadScript2.Runtime.Objects.Native;

/// <summary>
/// Implements the Interface for Native Dates
/// </summary>
public interface IBadDate : IBadNative
{
    /// <summary>
    ///     The Date Value
    /// </summary>
    new DateTimeOffset Value { get; }
}