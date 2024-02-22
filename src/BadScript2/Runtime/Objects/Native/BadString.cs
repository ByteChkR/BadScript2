using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native String
/// </summary>
public class BadString : BadNative<string>, IBadString, IComparable, IComparable<BadObject>, IComparable<IBadString>
{
    /// <summary>
    ///     The Prototype for the Native String Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("string");

    /// <summary>
    ///     Creates a new Native String
    /// </summary>
    /// <param name="value">The String Value</param>
    public BadString(string value) : base(value) { }

    /// <summary>
    ///     The Value of the Native String
    /// </summary>
    string IBadString.Value => Value;

    /// <summary>
    ///     Compares this String to another String
    /// </summary>
    /// <param name="obj">The String to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    /// <exception cref="Exception">Gets raised if the <paramref name="obj" /> is not a String</exception>
    public int CompareTo(object obj)
    {
        if (obj is BadObject o)
        {
            return CompareTo(o);
        }

        throw new Exception("Cannot compare string to non string");
    }

    /// <summary>
    ///     Compares this String to another String
    /// </summary>
    /// <param name="other">The String to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    /// <exception cref="Exception">Gets raised if the <paramref name="other" /> is not a String</exception>
    public int CompareTo(BadObject other)
    {
        if (other is IBadString str)
        {
            return CompareTo(str);
        }

        throw new Exception("Cannot compare string to non string");
    }

    /// <summary>
    ///     Compares this String to another String
    /// </summary>
    /// <param name="other">The String to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    public int CompareTo(IBadString other)
    {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }
}