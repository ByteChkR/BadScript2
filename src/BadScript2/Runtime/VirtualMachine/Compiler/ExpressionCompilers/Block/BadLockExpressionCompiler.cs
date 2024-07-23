using BadScript2.Parser.Expressions.Block.Lock;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadLockExpression" />.
/// </summary>
public class BadLockExpressionCompiler : BadExpressionCompiler<BadLockExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadLockExpression expression)
    {
        context.Compile(expression.LockExpression);
        context.Emit(BadOpCode.Dup, expression.Position);
        if (expression.Block.Any()) // Dont aquire lock if there are no expressions in the block
        {
            context.Emit(BadOpCode.AquireLock, expression.Position);
            context.Compile(expression.Block);
            context.Emit(BadOpCode.ReleaseLock, expression.Position);
        }
    }
}