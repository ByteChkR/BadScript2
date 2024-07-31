using BadScript2.Parser.Expressions.Block;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadUsingStatementExpression" />.
/// </summary>
/// c
public class BadUsingStatementExpressionCompiler : BadExpressionCompiler<BadUsingStatementExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadUsingStatementExpression expression)
    {
        context.Compile(expression.Expression);
        context.Emit(BadOpCode.AddDisposeFinalizer, expression.Position, expression.Name);
    }
}