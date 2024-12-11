namespace BadScript2.Runtime.Objects.Native;

public interface IBadTime : IBadNative
{
    /// <summary>
    ///     The TimeSpan Value
    /// </summary>
    new TimeSpan Value { get; }
}