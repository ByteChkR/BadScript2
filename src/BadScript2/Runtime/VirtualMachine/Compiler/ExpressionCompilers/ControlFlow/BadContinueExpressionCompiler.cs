using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
/// Compiles the <see cref="BadContinueExpression" />.
/// </summary>
public class BadContinueExpressionCompiler : BadExpressionCompiler<BadContinueExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadContinueExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Continue, expression.Position);
    }
}