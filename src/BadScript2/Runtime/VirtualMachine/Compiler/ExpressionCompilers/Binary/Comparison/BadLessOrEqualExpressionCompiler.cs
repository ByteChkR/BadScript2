using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <summary>
/// Compiles the <see cref="BadLessOrEqualExpression" />.
/// </summary>
public class BadLessOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadLessOrEqualExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLessOrEqualExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LessEquals, expression.Position);
    }
}