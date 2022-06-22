using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive
{
    public class BadNullToken : BadPrimitiveToken
    {
        public BadNullToken(BadSourcePosition position) : base(position) { }

        public override BadPrimitiveType Type => BadPrimitiveType.Null;
    }
}