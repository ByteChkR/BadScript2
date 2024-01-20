namespace BadScript2.Runtime.Objects.Types;

public class BadAbstractClassException : Exception
{
    public BadAbstractClassException(string className) : base(
        $"Class {className} is abstract and cannot be instantiated."
    ) { }
}