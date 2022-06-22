using BadScript2.Common;

namespace BadScript2.Reader.Token
{
    public abstract class BadToken
    {
        public readonly BadSourcePosition SourcePosition;

        protected BadToken(BadSourcePosition position)
        {
            SourcePosition = position;
        }

        public string Text => SourcePosition.Text;

        public override string ToString()
        {
            return Text;
        }
    }
}