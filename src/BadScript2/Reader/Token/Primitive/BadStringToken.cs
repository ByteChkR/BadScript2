using BadScript2.Common;
namespace BadScript2.Reader.Token.Primitive;

/// <summary>
///     Implements a Token that represents a String
/// </summary>
public class BadStringToken : BadPrimitiveToken
{
	/// <summary>
	///     Constructor for a String Token
	/// </summary>
	/// <param name="str">The String value(including the Quotes)</param>
	/// <param name="position">The Position in the Source</param>
	public BadStringToken(string str, BadSourcePosition position) : base(position)
    {
        Value = str;
    }

    /// <inheritdoc />
    public override BadPrimitiveType Type => BadPrimitiveType.String;

    /// <summary>
    ///     The Value of the Token
    /// </summary>
    public string Value { get; }
}