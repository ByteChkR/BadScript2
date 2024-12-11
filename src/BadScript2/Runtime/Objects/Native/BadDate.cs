using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

public class BadDate : BadNative<DateTimeOffset>, IBadDate
{
    public static BadDate Now => new BadDate(DateTimeOffset.Now);
    public static BadDate UtcNow => new BadDate(DateTimeOffset.Now);
    public static BadClassPrototype Prototype = new BadDatePrototype();
    
    public BadDate(DateTimeOffset value) : base(value)
    {
    }
}