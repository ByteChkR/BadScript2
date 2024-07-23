using BadScript2.Parser.Expressions.Constant;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <summary>
///     Compiles the <see cref="BadConstantExpression" />.
/// </summary>
public class BadConstantExpressionCompiler : BadExpressionCompiler<BadConstantExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadConstantExpression expression)
    {
        context.Emit(BadOpCode.Push, expression.Position, expression.Value);
    }
}