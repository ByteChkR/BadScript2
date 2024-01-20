/// <summary>
/// Contains Runtime Type Objects
/// </summary>
namespace BadScript2.Runtime.Objects.Types;

/// <summary>
/// Exception that is thrown when a Abstract Class is instantiated.
/// </summary>
public class BadAbstractClassException : Exception
{
    /// <summary>
    /// Creates a new Exception
    /// </summary>
    /// <param name="className">The Name of the Class</param>
    public BadAbstractClassException(string className) : base(
        $"Class {className} is abstract and cannot be instantiated."
    ) { }
}