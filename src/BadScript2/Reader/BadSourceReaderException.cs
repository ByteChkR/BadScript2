using BadScript2.Common;

namespace BadScript2.Reader
{
    public class BadSourceReaderException : BadParseException
    {
        public BadSourceReaderException(string message, BadSourcePosition position) : base(message, position) { }

        public BadSourceReaderException(string message, BadSourcePosition position, Exception inner) : base(
            message,
            position,
            inner
        ) { }
    }
}