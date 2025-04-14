using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
/// Implements the Object for Native Dates
/// </summary>
public class BadDate : BadNative<DateTimeOffset>, IBadDate
{
    /// <summary>
    /// DateTimeOffset Now
    /// </summary>
    public static BadDate Now => new BadDate(DateTimeOffset.Now);
    /// <summary>
    /// DateTimeOffset UtcNow
    /// </summary>
    public static BadDate UtcNow => new BadDate(DateTimeOffset.Now);
    /// <summary>
    /// The Type of the Object
    /// </summary>
    public static BadClassPrototype Prototype = new BadDatePrototype();
    
    /// <summary>
    /// Creates a new BadDate Object
    /// </summary>
    /// <param name="value">The DateTimeOffset Value</param>
    public BadDate(DateTimeOffset value) : base(value)
    {
    }
}