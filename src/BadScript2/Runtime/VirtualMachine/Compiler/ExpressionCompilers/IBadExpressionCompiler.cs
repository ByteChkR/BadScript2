using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Defines a Compiler for a specific <see cref="BadExpression" />.
/// </summary>
public interface IBadExpressionCompiler
{
    IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadExpression expression);
}