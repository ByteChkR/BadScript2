using BadScript2.Parser.Expressions;
using BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadInstanceOfExpressionCompiler : BadBinaryExpressionCompiler<BadInstanceOfExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInstanceOfExpression expression)
    {
        yield return new BadInstruction(BadOpCode.InstanceOf, expression.Position);
    }
}