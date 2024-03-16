using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <summary>
///     Compiles the <see cref="BadPreIncrementExpression" />.
/// </summary>
public class BadPreIncrementExpressionCompiler : BadExpressionCompiler<BadPreIncrementExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadPreIncrementExpression expression)
    {
        context.Compile(expression.Right);
        context.Emit(BadOpCode.PreInc, expression.Position);
    }
}