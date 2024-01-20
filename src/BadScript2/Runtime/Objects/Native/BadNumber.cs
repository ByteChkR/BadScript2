using System.Globalization;

using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Number
/// </summary>
public class BadNumber : BadNative<decimal>, IBadNumber, IComparable, IComparable<BadObject>, IComparable<IBadNumber>
{
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("num");

    /// <summary>
    ///     Creates a new Native Number
    /// </summary>
    /// <param name="value">The Number Value</param>
    public BadNumber(decimal value) : base(value) { }

    decimal IBadNumber.Value => Value;

    public int CompareTo(object obj)
    {
        if (obj is BadObject o)
        {
            return CompareTo(o);
        }

        throw new Exception("Cannot compare number to non number");
    }

    public int CompareTo(BadObject other)
    {
        if (other is IBadNumber num)
        {
            return CompareTo(num);
        }

        throw new Exception("Cannot compare number to non number");
    }

    public int CompareTo(IBadNumber other)
    {
        return Value.CompareTo(other.Value);
    }

    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    public override string ToSafeString(List<BadObject> done)
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}