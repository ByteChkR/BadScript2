using BadScript2.Runtime.Objects.Types;

namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Boolean
/// </summary>
public class BadBoolean : BadNative<bool>, IBadBoolean
{
	/// <summary>
	///     Creates a new Native Boolean
	/// </summary>
	/// <param name="value">The Boolean Value</param>
	public BadBoolean(bool value) : base(value) { }

	bool IBadBoolean.Value => Value;

	private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("bool");
	public override BadClassPrototype GetPrototype()
	{
		return s_Prototype;
	}
}
