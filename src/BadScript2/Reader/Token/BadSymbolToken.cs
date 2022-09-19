using BadScript2.Common;

namespace BadScript2.Reader.Token;

/// <summary>
/// Implements a Symbol Token
/// </summary>
public class BadSymbolToken : BadToken
{
    /// <summary>
    /// Constructor for the token
    /// </summary>
    /// <param name="position">The Source Position of the Token</param>
    public BadSymbolToken(BadSourcePosition position) : base(position) { }
}