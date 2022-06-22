using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive;

public abstract class BadPrimitiveToken : BadToken
{
    protected BadPrimitiveToken(BadSourcePosition position) : base(position) { }

    public abstract BadPrimitiveType Type { get; }
}