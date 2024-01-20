using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
/// Compiles the <see cref="BadGreaterThanExpression" />.
/// </summary>
public class BadGreaterExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterThanExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadGreaterThanExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Greater, expression.Position);
    }
}