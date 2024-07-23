using BadScript2.Common;
namespace BadScript2.Reader.Token;

/// <summary>
///     Base Class for All tokens.
/// </summary>
public abstract class BadToken
{
	/// <summary>
	///     Constructor for the Token
	/// </summary>
	/// <param name="position">The Source Position of the Token</param>
	protected BadToken(BadSourcePosition position)
    {
        SourcePosition = position;
    }

	/// <summary>
	///     The Source Position of the Token
	/// </summary>
	public BadSourcePosition SourcePosition { get; }

	/// <summary>
	///     The Text Representation of the Token
	/// </summary>
	public string Text => SourcePosition.Text;

	/// <summary>
	///     The String Representation of the Token
	/// </summary>
	/// <returns>String Representation</returns>
	public override string ToString()
    {
        return Text;
    }
}