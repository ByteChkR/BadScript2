namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements the Interface for Native Boolean
/// </summary>
public interface IBadBoolean : IBadNative
{
    /// <summary>
    ///     The Boolean Value
    /// </summary>
    new bool Value { get; }
}
