using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

public class BadNumber : BadNative<decimal>, IBadNumber
{
    public BadNumber(decimal value) : base(value) { }
    decimal IBadNumber.Value => Value;

    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.GetNative("num");
    }
}