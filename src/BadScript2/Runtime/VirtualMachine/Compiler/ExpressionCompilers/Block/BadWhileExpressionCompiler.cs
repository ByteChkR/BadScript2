using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadWhileExpression" />.
/// </summary>
public class BadWhileExpressionCompiler : BadExpressionCompiler<BadWhileExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadWhileExpression expression)
    {
        int start = context.InstructionCount;
        context.Compile(expression.Condition);
        int endJump = context.EmitEmpty();
        int loopScopeStart = context.InstructionCount;

        //Break And continue pointers are relative to the "CreateScope" instruction
        //If break instruction is encountered
        //  Jump to loopScopeStart + offset to after loop(the destroy scope instruction is omitted because the vm handles the scopes)
        context.Emit(BadOpCode.CreateScope,
                     expression.Position,
                     "WhileScope",
                     BadObject.Null,
                     BadScopeFlags.Breakable | BadScopeFlags.Continuable
                    );
        int setBreakInstruction = context.EmitEmpty();
        int setContinueInstruction = context.EmitEmpty();
        context.Compile(expression.Body);
        context.Emit(BadOpCode.DestroyScope, expression.Position);
        //Jump back up to the loop condition
        int continueJump = context.InstructionCount;
        context.Emit(BadOpCode.JumpRelative, expression.Position, start - context.InstructionCount - 1);

        //Set the end jump to the end of the loop
        context.ResolveEmpty(endJump,
                             BadOpCode.JumpRelativeIfFalse,
                             expression.Position,
                             context.InstructionCount - endJump - 1
                            );

        //address to the end of the loop(relative to the create scope instruction)
        context.ResolveEmpty(setBreakInstruction,
                             BadOpCode.SetBreakPointer,
                             expression.Position,
                             context.InstructionCount - loopScopeStart - 1
                            );

        //Address to start of the condition(relative to the start of the loop)
        context.ResolveEmpty(setContinueInstruction,
                             BadOpCode.SetContinuePointer,
                             expression.Position,
                             continueJump - loopScopeStart - 1
                            );
    }
}