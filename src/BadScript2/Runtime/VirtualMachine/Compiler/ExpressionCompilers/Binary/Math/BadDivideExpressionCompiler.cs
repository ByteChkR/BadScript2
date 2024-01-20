using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadDivideExpressionCompiler : BadBinaryExpressionCompiler<BadDivideExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadDivideExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Div, expression.Position);
    }
}