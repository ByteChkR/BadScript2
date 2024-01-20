using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadContinueExpressionCompiler : BadExpressionCompiler<BadContinueExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadContinueExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Continue, expression.Position);
    }
}