using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive;

/// <summary>
///     Base class for all primitive tokens
/// </summary>
public abstract class BadPrimitiveToken : BadToken
{
    /// <summary>
    ///     Constructor for primitive tokens
    /// </summary>
    /// <param name="position">The Source Position of the Token</param>
    protected BadPrimitiveToken(BadSourcePosition position) : base(position) { }

    /// <summary>
    ///     The Primitive Type of the Token
    /// </summary>
    public abstract BadPrimitiveType Type { get; }
}
