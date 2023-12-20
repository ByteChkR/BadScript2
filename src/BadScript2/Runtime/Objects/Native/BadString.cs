using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native String
/// </summary>
public class BadString : BadNative<string>, IBadString, IComparable, IComparable<BadObject>, IComparable<IBadString>
{
	private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("string");

	/// <summary>
	///     Creates a new Native String
	/// </summary>
	/// <param name="value">The String Value</param>
	public BadString(string value) : base(value) { }

	string IBadString.Value => Value;

	public int CompareTo(object obj)
	{
		if (obj is BadObject o)
		{
			return CompareTo(o);
		}

		throw new Exception("Cannot compare string to non string");
	}

	public int CompareTo(BadObject other)
	{
		if (other is IBadString str)
		{
			return CompareTo(str);
		}

		throw new Exception("Cannot compare string to non string");
	}

	public int CompareTo(IBadString other)
	{
		return string.Compare(Value, other.Value, StringComparison.Ordinal);
	}

	public override BadClassPrototype GetPrototype()
	{
		return s_Prototype;
	}
}
