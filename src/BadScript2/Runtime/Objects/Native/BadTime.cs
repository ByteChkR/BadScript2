using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
/// Implements the Object for Native TimeSpans
/// </summary>
public class BadTime : BadNative<TimeSpan>, IBadTime
{
    /// <summary>
    /// Zero TimeSpan
    /// </summary>
    public static BadTime Zero => new BadTime(TimeSpan.Zero);
    /// <summary>
    /// The Type of the Object
    /// </summary>
    public static BadClassPrototype Prototype = new BadTimePrototype();
    /// <summary>
    /// Creates a new BadTime Object
    /// </summary>
    /// <param name="value">The TimeSpan Value</param>
    public BadTime(TimeSpan value) : base(value)
    {
    }
}