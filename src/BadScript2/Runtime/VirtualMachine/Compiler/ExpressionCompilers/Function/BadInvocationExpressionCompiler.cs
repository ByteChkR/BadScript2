using BadScript2.Parser.Expressions.Function;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

/// <summary>
///     Compiles the <see cref="BadInvocationExpression" />.
/// </summary>
public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadInvocationExpression expression)
    {
        context.Compile(expression.Arguments, false);
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Invoke, expression.Position, expression.ArgumentCount);
    }
}