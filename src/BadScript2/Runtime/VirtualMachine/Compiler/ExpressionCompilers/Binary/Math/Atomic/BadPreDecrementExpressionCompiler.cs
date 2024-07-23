using BadScript2.Parser.Expressions.Binary.Math.Atomic;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPreDecrementExpression" />.
/// </summary>
public class BadPreDecrementExpressionCompiler : BadExpressionCompiler<BadPreDecrementExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadPreDecrementExpression expression)
    {
        context.Compile(expression.Right);
        context.Emit(BadOpCode.PreDec, expression.Position);
    }
}