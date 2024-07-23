using BadScript2.Parser.Expressions.Binary;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadUnaryUnpackExpression" />.
/// </summary>
public class BadUnaryUnpackExpressionCompiler : BadExpressionCompiler<BadUnaryUnpackExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadUnaryUnpackExpression expression)
    {
        context.Compile(expression.Right);
        context.Emit(BadOpCode.UnaryUnpack, expression.Position);
    }
}