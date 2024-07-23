using BadScript2.Parser.Expressions;
namespace BadScript2.Runtime.VirtualMachine.Compiler;

/// <summary>
///     Gets thrown when a Compiler is not able to compile a specific <see cref="BadExpression" />.
/// </summary>
public class BadCompilerException : Exception
{
    /// <summary>
    ///     Creates a new <see cref="BadCompilerException" /> instance.
    /// </summary>
    /// <param name="message">The error message.</param>
    public BadCompilerException(string message) : base(message) { }
}