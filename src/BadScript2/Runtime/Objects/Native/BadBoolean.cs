using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native
{
    public class BadBoolean : BadNative<bool>, IBadBoolean
    {
        public BadBoolean(bool value) : base(value) { }
        bool IBadBoolean.Value => Value;

        public override BadClassPrototype GetPrototype()
        {
            return BadNativeClassBuilder.GetNative("bool");
        }
    }
}