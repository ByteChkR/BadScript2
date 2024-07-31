using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadForExpression" />.
/// </summary>
public class BadForExpressionCompiler : BadExpressionCompiler<BadForExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadForExpression expression)
    {
        context.Emit(BadOpCode.CreateScope, expression.Position, "ForLoop", BadObject.Null);
        context.Compile(expression.VarDef);
        int conditionJump = context.InstructionCount;
        context.Compile(expression.Condition);
        int endJump = context.EmitEmpty();
        int loopScopeStart = context.InstructionCount;

        context.Emit(BadOpCode.CreateScope,
                     expression.Position,
                     "ForLoopBody",
                     BadObject.Null,
                     BadScopeFlags.Breakable | BadScopeFlags.Continuable
                    );
        int setBreakInstruction = context.EmitEmpty();
        int setContinueInstruction = context.EmitEmpty();
        context.Compile(expression.Body);
        context.Emit(BadOpCode.DestroyScope, expression.Position);
        int continueJump = context.InstructionCount;
        context.Compile(expression.VarIncrement);
        context.Emit(BadOpCode.JumpRelative, expression.Position, conditionJump - context.InstructionCount - 1);
        int endJumpPos = context.InstructionCount - endJump - 1;
        context.ResolveEmpty(endJump, BadOpCode.JumpRelativeIfFalse, expression.Position, endJumpPos);
        context.Emit(BadOpCode.DestroyScope, expression.Position);

        context.ResolveEmpty(setBreakInstruction,
                             BadOpCode.SetBreakPointer,
                             expression.Position,
                             context.InstructionCount - loopScopeStart
                            );

        context.ResolveEmpty(setContinueInstruction,
                             BadOpCode.SetContinuePointer,
                             expression.Position,
                             continueJump - loopScopeStart - 1
                            );
    }
}