namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements the Interface for Native Strings
/// </summary>
public interface IBadString : IBadNative
{
    /// <summary>
    ///     The String Value
    /// </summary>
    new string Value { get; }
}
