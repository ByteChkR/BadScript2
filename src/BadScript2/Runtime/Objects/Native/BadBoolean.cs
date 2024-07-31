using BadScript2.Runtime.Objects.Types;

/// <summary>
/// Contains the Native Runtime Objects
/// </summary>
namespace BadScript2.Runtime.Objects.Native;

/// <summary>
///     Implements a Native Boolean
/// </summary>
public class BadBoolean : BadNative<bool>, IBadBoolean
{
    /// <summary>
    ///     The Prototype for the Native Boolean Object
    /// </summary>
    private static readonly BadClassPrototype s_Prototype = BadNativeClassBuilder.GetNative("bool");

    /// <summary>
    ///     Creates a new Native Boolean
    /// </summary>
    /// <param name="value">The Boolean Value</param>
    public BadBoolean(bool value) : base(value) { }

#region IBadBoolean Members

    /// <inheritdoc />
    bool IBadBoolean.Value => Value;

#endregion

    /// <inheritdoc />
    public override BadClassPrototype GetPrototype()
    {
        return s_Prototype;
    }
}