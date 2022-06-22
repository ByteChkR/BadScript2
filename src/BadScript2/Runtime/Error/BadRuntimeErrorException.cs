namespace BadScript2.Runtime.Error
{
    public class BadRuntimeErrorException : Exception
    {
        public BadRuntimeErrorException(BadRuntimeError? error) : base(error?.ToString() ?? "<no error>") { }
    }
}