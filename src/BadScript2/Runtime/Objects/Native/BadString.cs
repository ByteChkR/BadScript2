using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

public class BadString : BadNative<string>, IBadString
{
    public BadString(string value) : base(value) { }
    string IBadString.Value => Value;

    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.GetNative("string");
    }
}