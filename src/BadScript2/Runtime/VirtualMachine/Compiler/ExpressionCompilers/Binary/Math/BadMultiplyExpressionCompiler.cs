using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <summary>
/// Compiles the <see cref="BadMultiplyExpression" />.
/// </summary>
public class BadMultiplyExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadMultiplyExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Mul, expression.Position);
    }
}