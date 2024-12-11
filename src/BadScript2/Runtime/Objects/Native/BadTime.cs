using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

public class BadTime : BadNative<TimeSpan>, IBadTime
{
    public static BadTime Zero => new BadTime(TimeSpan.Zero);
    public static BadClassPrototype Prototype = new BadTimePrototype();
    public BadTime(TimeSpan value) : base(value)
    {
    }
}