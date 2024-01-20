using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadAddAssignExpressionCompiler : BadBinaryExpressionCompiler<BadAddAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AddAssign, expression.Position);
    }
}