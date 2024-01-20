using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <summary>
/// Compiles the <see cref="BadMultiplyAssignExpression" />.
/// </summary>
public class BadMultiplyAssignExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyAssignExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadMultiplyAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.MulAssign, expression.Position);
    }
}