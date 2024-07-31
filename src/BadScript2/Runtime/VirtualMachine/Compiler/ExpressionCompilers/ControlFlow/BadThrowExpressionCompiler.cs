using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <summary>
///     Compiles the <see cref="BadThrowExpression" />.
/// </summary>
public class BadThrowExpressionCompiler : BadExpressionCompiler<BadThrowExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadThrowExpression expression)
    {
        context.Compile(expression.Right);
        context.Emit(BadOpCode.Throw, expression.Position);
    }
}