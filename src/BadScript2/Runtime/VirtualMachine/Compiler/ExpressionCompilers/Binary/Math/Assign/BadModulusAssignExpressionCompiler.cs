using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadModulusAssignExpressionCompiler : BadBinaryExpressionCompiler<BadModulusAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadModulusAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.ModAssign, expression.Position);
    }
}