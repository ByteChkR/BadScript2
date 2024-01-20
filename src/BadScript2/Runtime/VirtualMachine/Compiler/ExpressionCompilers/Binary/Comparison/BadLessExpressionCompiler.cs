using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadLessExpressionCompiler : BadBinaryExpressionCompiler<BadLessThanExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLessThanExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Less, expression.Position);
    }
}