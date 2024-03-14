using BadScript2.Parser.Expressions;

/// <summary>
/// Contains Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <summary>
///     Compiles the <see cref="BadDeleteExpression" />.
/// </summary>
public class BadDeleteExpressionCompiler : BadExpressionCompiler<BadDeleteExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadDeleteExpression expression)
    {
        context.Compile(expression.Expression);
        context.Emit(BadOpCode.Delete, expression.Position);
    }
}