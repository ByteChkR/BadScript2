using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

public interface IBadExpressionCompiler
{
    IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadExpression expression);
}