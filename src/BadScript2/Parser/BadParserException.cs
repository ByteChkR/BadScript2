using BadScript2.Common;

namespace BadScript2.Parser
{
    public class BadParserException : BadParseException
    {
        public BadParserException(
            string message,
            BadSourcePosition position) : base(message, position) { }

        public BadParserException(
            string message,
            BadSourcePosition position,
            Exception inner) :
            base(message, position, inner) { }
    }
}