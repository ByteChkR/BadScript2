using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native
{
    /// <summary>
    ///     Implements a Native String
    /// </summary>
    public class BadString : BadNative<string>, IBadString
    {
        /// <summary>
        ///     Creates a new Native String
        /// </summary>
        /// <param name="value">The String Value</param>
        public BadString(string value) : base(value) { }

        string IBadString.Value => Value;

        public override BadClassPrototype GetPrototype()
        {
            return BadNativeClassBuilder.GetNative("string");
        }
    }
}