using BadScript2.Parser.Expressions.Block.Lock;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadLockExpression" />.
/// </summary>
public class BadLockExpressionCompiler : BadExpressionCompiler<BadLockExpression>
{
    private const string LockVariableName = "~LOCK~";

    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadLockExpression expression)
    {
        context.Compile(expression.LockExpression);
        context.Emit(BadOpCode.Dup, expression.Position);

        if (expression.Block.Any()) // Dont aquire lock if there are no expressions in the block
        {
            context.Emit(BadOpCode.CreateScope, expression.Position, "LOCK_SCOPE", BadObject.Null);
            context.Emit(BadOpCode.DefVar, expression.Position, LockVariableName, true);
            context.Emit(BadOpCode.Swap, expression.Position);
            context.Emit(BadOpCode.Assign, expression.Position);
            context.Emit(BadOpCode.AquireLock, expression.Position);
            context.Compile(expression.Block);
            context.Emit(BadOpCode.LoadVar, expression.Position, LockVariableName, 0);
            context.Emit(BadOpCode.ReleaseLock, expression.Position);
            context.Emit(BadOpCode.DestroyScope, expression.Position);
        }
    }
}