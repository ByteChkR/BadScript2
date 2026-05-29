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

    // -------------------------------------------------------------------------
    // Phase C1 – Integer Cache (escape analysis: avoid allocation for common
    //            integer values that do not need unique identity)
    // -------------------------------------------------------------------------

    /// <summary>Smallest cached integer value.</summary>
    private const int IntCacheMin = -128;

    /// <summary>Largest cached integer value.</summary>
    private const int IntCacheMax = 1024;

    private static readonly BadNumber[] s_IntCache;

    static BadNumber()
    {
        s_IntCache = new BadNumber[IntCacheMax - IntCacheMin + 1];

        for (int i = 0; i < s_IntCache.Length; i++)
        {
            s_IntCache[i] = new BadNumber(IntCacheMin + i);
        }
    }

    /// <summary>
    ///     Returns a cached <see cref="BadNumber"/> for integer values in
    ///     [<see cref="IntCacheMin"/>, <see cref="IntCacheMax"/>], otherwise
    ///     allocates a new instance.  Always use this instead of <c>new BadNumber(d)</c>
    ///     or the implicit <c>decimal → BadObject</c> operator in hot paths.
    /// </summary>
    public static BadNumber Get(decimal d)
    {
        if (d >= IntCacheMin && d <= IntCacheMax)
        {
            // Cheap integer check: no remainder means it is a whole number
            decimal floor = Math.Truncate(d);

            if (floor == d)
            {
                return s_IntCache[(int)d - IntCacheMin];
            }
        }

        return new BadNumber(d);
    }

    // -------------------------------------------------------------------------

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