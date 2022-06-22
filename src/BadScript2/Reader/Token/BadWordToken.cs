using BadScript2.Common;

namespace BadScript2.Reader.Token
{
    public class BadWordToken : BadToken
    {
        public BadWordToken(BadSourcePosition position) : base(position) { }

        public static BadWordToken MakeWord(string s)
        {
            return new BadWordToken(BadSourcePosition.FromSource(s, 0, s.Length));
        }

        public static implicit operator BadWordToken(string s)
        {
            return MakeWord(s);
        }
    }
}