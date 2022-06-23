using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive;

public class BadStringToken : BadPrimitiveToken
{
    public BadStringToken(string str, BadSourcePosition position) : base(position)
    {
        Value = str;
    }

    public override BadPrimitiveType Type => BadPrimitiveType.String;

    public string Value { get; }
}