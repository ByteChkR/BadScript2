using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadExponentiationExpressionCompiler : BadBinaryExpressionCompiler<BadExponentiationExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadExponentiationExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Exp, expression.Position);
    }
}