using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPostIncrementExpression" />.
/// </summary>
public class BadPostIncrementExpressionCompiler : BadExpressionCompiler<BadPostIncrementExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadPostIncrementExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.PostInc, expression.Position);
    }
}