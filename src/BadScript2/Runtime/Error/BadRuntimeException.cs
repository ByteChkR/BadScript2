using BadScript2.Common;

namespace BadScript2.Runtime.Error
{
    public class BadRuntimeException : Exception
    {
        public BadRuntimeException(string message) : base(message) { }

        public BadRuntimeException(string message, BadSourcePosition position) : this(
            $"{message} at {position.GetPositionInfo()}"
        ) { }
    }
}