using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Defines a Compiler for a specific <see cref="BadExpression" />.
/// </summary>
public interface IBadExpressionCompiler
{
    /// <summary>
    /// Compiles the given <see cref="BadExpression" /> into a set of <see cref="BadInstruction" />s.
    /// </summary>
    /// <param name="compiler">The <see cref="BadCompiler" /> instance to use.</param>
    /// <param name="expression">The <see cref="BadExpression" /> to compile.</param>
    /// <returns>List of <see cref="BadInstruction" />s.</returns>
    IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadExpression expression);
}