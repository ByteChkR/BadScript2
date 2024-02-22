using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
///     Compiles the <see cref="BadInequalityExpression" />.
/// </summary>
public class BadInequalityExpressionCompiler : BadBinaryExpressionCompiler<BadInequalityExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInequalityExpression expression)
    {
        yield return new BadInstruction(BadOpCode.NotEquals, expression.Position);
    }
}