using System.Collections;
using BadScript2.Runtime.Interop;
using BadScript2.Runtime.Objects.Types;
using BadScript2.Utility.Linq;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native String
/// </summary>
public class BadString : BadNative<string>, IBadString, IComparable, IComparable<BadObject>, IComparable<IBadString>, IBadEnumerable
{
    /// <summary>
    ///     The Prototype for the Native String Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("string");

    public static readonly BadString Empty = new BadString(string.Empty);

    /// <summary>
    ///     Creates a new Native String
    /// </summary>
    /// <param name="value">The String Value</param>
    public BadString(string value) : base(value) { }

#region IBadString Members

    /// <summary>
    ///     The Value of the Native String
    /// </summary>
    string IBadString.Value => Value;

#endregion

#region IComparable Members

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

#endregion

#region IComparable<BadObject> Members

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

#endregion

#region IComparable<IBadString> Members

    /// <summary>
    ///     Compares this String to another String
    /// </summary>
    /// <param name="other">The String to compare to</param>
    /// <returns>0 if equal, -1 if this is smaller, 1 if this is bigger</returns>
    public int CompareTo(IBadString other)
    {
        return string.Compare(Value, other.Value, StringComparison.Ordinal);
    }

#endregion

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }

    /// <summary>
    /// Returns the String Value character by character
    /// </summary>
    /// <returns>Enumerator for the String</returns>
    public IEnumerator<BadObject> GetEnumerator()
    {
        return Value.Select(x=>(BadObject)x).GetEnumerator();
    }

    
    /// <summary>
    /// Returns the String Value character by character
    /// </summary>
    /// <returns>Enumerator for the String</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}