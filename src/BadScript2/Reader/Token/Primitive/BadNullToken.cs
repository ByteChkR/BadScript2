using BadScript2.Common;
namespace BadScript2.Reader.Token.Primitive;

/// <summary>
///     Implements a Null token
/// </summary>
public class BadNullToken : BadPrimitiveToken
{
	/// <summary>
	///     Constructor for the Null token
	/// </summary>
	/// <param name="position">Source Position of the Token</param>
	public BadNullToken(BadSourcePosition position) : base(position) { }

    /// <inheritdoc />
    public override BadPrimitiveType Type => BadPrimitiveType.Null;
}