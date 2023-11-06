namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements the Interface for Native Numbers
/// </summary>
public interface IBadNumber : IBadNative
{
    /// <summary>
    ///     The Number Value
    /// </summary>
    new decimal Value { get; }
}
