using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadGreaterOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterOrEqualExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadGreaterOrEqualExpression expression)
    {
        yield return new BadInstruction(BadOpCode.GreaterEquals, expression.Position);
    }
}