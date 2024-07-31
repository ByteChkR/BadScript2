using System.Globalization;

using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Number
/// </summary>
public class BadNumber : BadNative<decimal>, IBadNumber, IComparable, IComparable<BadObject>, IComparable<IBadNumber>
{
    /// <summary>
    ///     The Prototype for the Native Number Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("num");

    /// <summary>
    ///     Creates a new Native Number
    /// </summary>
    /// <param name="value">The Number Value</param>
    public BadNumber(decimal value) : base(value) { }

#region IBadNumber Members

    /// <inheritdoc />
    decimal IBadNumber.Value => Value;

#endregion

#region IComparable Members

    /// <summary>
    ///     Compares this Number to another Number
    /// </summary>
    /// <param name="obj">The Number to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    /// <exception cref="Exception">Gets raised if the <paramref name="obj" /> is not a Number</exception>
    public int CompareTo(object obj)
    {
        if (obj is BadObject o)
        {
            return CompareTo(o);
        }

        throw new Exception("Cannot compare number to non number");
    }

#endregion

#region IComparable<BadObject> Members

    /// <summary>
    ///     Compares this Number to another Number
    /// </summary>
    /// <param name="other">The Number to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    /// <exception cref="Exception">Gets raised if the <paramref name="other" /> is not a Number</exception>
    public int CompareTo(BadObject other)
    {
        if (other is IBadNumber num)
        {
            return CompareTo(num);
        }

        throw new Exception("Cannot compare number to non number");
    }

#endregion

#region IComparable<IBadNumber> Members

    /// <summary>
    ///     Compares this Number to another Number
    /// </summary>
    /// <param name="other">The Number to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    public int CompareTo(IBadNumber other)
    {
        return Value.CompareTo(other.Value);
    }

#endregion

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <inheritdoc />
    public override string ToSafeString(List<BadObject> done)
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}