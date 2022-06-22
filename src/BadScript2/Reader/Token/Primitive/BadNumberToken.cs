using BadScript2.Common;

namespace BadScript2.Reader.Token.Primitive
{
    public class BadNumberToken : BadPrimitiveToken
    {
        public BadNumberToken(BadSourcePosition position) : base(position) { }

        public override BadPrimitiveType Type => BadPrimitiveType.Number;

        public decimal Value => decimal.Parse(Text);
    }
}