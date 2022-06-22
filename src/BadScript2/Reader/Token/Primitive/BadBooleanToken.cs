using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive
{
    public class BadBooleanToken : BadPrimitiveToken
    {
        public BadBooleanToken(BadSourcePosition position) : base(position) { }

        public override BadPrimitiveType Type => BadPrimitiveType.Boolean;

        public bool Value => bool.Parse(Text);
    }
}