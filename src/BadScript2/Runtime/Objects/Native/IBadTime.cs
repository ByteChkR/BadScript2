namespace BadScript2.Runtime.Objects.Native;

/// <summary>
/// Implements the Interface for Native TimeSpans
/// </summary>
public interface IBadTime : IBadNative
{
    /// <summary>
    ///     The TimeSpan Value
    /// </summary>
    new TimeSpan Value { get; }
}