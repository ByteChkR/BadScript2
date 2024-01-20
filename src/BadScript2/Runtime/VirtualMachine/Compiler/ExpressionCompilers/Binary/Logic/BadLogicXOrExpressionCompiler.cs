using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadLogicXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicXOrExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicXOrExpression expression)
    {
        yield return new BadInstruction(BadOpCode.XOr, expression.Position);
    }
}