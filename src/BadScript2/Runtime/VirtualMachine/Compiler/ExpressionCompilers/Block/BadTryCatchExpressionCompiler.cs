using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadTryCatchExpression" />.
/// </summary>
public class BadTryCatchExpressionCompiler : BadExpressionCompiler<BadTryCatchExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadTryCatchExpression expression)
    {
        context.Emit(BadOpCode.CreateScope, expression.Position, "TryScope", BadObject.Null, BadScopeFlags.CaptureThrow);
        int setThrowInstruction = context.EmitEmpty();
        context.Compile(expression.TryExpressions);
        context.Emit(BadOpCode.DestroyScope, expression.Position);
        int jumpToEnd = context.EmitEmpty();
        int catchStart = context.InstructionCount;
        if (expression.CatchExpressions.Any()) // If there are catch expressions, compile them
        {
            context.Emit(BadOpCode.CreateScope, expression.Position, "CatchScope", BadObject.Null);
            context.Emit(BadOpCode.DefVar, expression.Position, expression.ErrorName, true);
            context.Emit(BadOpCode.Swap, expression.Position);
            context.Emit(BadOpCode.Assign, expression.Position);
            context.Compile(expression.CatchExpressions);
            context.Emit(BadOpCode.DestroyScope, expression.Position);
        }
        else
        {
            // If there are no catch expressions, we need to clean up the exception from the stack
            context.Emit(BadOpCode.Pop, expression.Position);
        }
        context.ResolveEmpty(setThrowInstruction, BadOpCode.SetThrowPointer, expression.Position, catchStart - 1);
        context.ResolveEmpty(jumpToEnd, BadOpCode.JumpRelative, expression.Position, context.InstructionCount - catchStart);
        if(expression.FinallyExpressions.Any()) // If there are finally expressions, compile them
        {
            context.Emit(BadOpCode.CreateScope, expression.Position, "FinallyScope", BadObject.Null);
            context.Compile(expression.FinallyExpressions);
            context.Emit(BadOpCode.DestroyScope, expression.Position);
        }
        
    }
}