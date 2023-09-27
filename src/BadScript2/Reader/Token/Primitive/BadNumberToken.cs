using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive;

/// <summary>
///     Implements a Token that represents a Number
/// </summary>
public class BadNumberToken : BadPrimitiveToken
{
	/// <summary>
	///     Constructs a new Number Token
	/// </summary>
	/// <param name="position">Source Position of the Token</param>
	public BadNumberToken(BadSourcePosition position) : base(position) { }

    public override BadPrimitiveType Type => BadPrimitiveType.Number;

    /// <summary>
    ///     The Value of the Token
    /// </summary>
    public decimal Value => decimal.Parse(Text);
}