using BadScript2.Parser.Expressions.ControlFlow;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
///     Compiles the <see cref="BadContinueExpression" />.
/// </summary>
public class BadContinueExpressionCompiler : BadExpressionCompiler<BadContinueExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadContinueExpression expression)
    {
        context.Emit(BadOpCode.Continue, expression.Position);
    }
}