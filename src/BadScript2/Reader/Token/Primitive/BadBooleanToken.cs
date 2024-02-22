using BadScript2.Common;

/// <summary>
/// Contains the Primitive Tokens for the BadScript2 Language
/// </summary>
namespace BadScript2.Reader.Token.Primitive;

/// <summary>
///     Implements a Token that represents a Boolean
/// </summary>
public class BadBooleanToken : BadPrimitiveToken
{
	/// <summary>
	///     Constructs a new Boolean Token
	/// </summary>
	/// <param name="position">Source Position of the Token</param>
	public BadBooleanToken(BadSourcePosition position) : base(position) { }

	/// <inheritdoc />
	public override BadPrimitiveType Type => BadPrimitiveType.Boolean;

	/// <summary>
	///     The Value of the Token
	/// </summary>
	public bool Value => bool.Parse(Text);
}