using System.Globalization;

using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Number
/// </summary>
public class BadNumber : BadNative<decimal>, IBadNumber
{
    /// <summary>
    ///     Creates a new Native Number
    /// </summary>
    /// <param name="value">The Number Value</param>
    public BadNumber(decimal value) : base(value) { }

    decimal IBadNumber.Value => Value;

    public override BadClassPrototype GetPrototype()
    {
        return BadNativeClassBuilder.GetNative("num");
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}