using BadScript2.Runtime;
using BadScript2.Parser.Expressions.Block.Loop;
using BadScript2.Runtime.Objects;

/// <summary>
/// Contains Block Expression Compilers
/// </summary>
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadForEachExpression" />.
/// </summary>
public class BadForEachExpressionCompiler : BadExpressionCompiler<BadForEachExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadForEachExpression expression)
    {
        context.Compile(expression.Target);
        context.Emit(BadOpCode.GetEnumerator, expression.Position);

        context.Emit(BadOpCode.CreateScope,
                     expression.Position,
                     "FOREACH_ENUMERATOR_SCOPE",
                     BadObject.Null
                    );
        context.Emit(BadOpCode.DefVar, expression.Position, "~ENUMERATOR~", true);
        context.Emit(BadOpCode.Swap, expression.Position);
        context.Emit(BadOpCode.Assign, expression.Position);

        int loopConditionStart = context.InstructionCount;
        context.Emit(BadOpCode.BeginLoop, expression.Position);
        context.Emit(BadOpCode.LoadVar, expression.Position, "~ENUMERATOR~", 0);
        context.Emit(BadOpCode.MoveNext, expression.Position);
        int endJump = context.EmitEmpty();

        context.Emit(BadOpCode.CreateScope,
                     expression.Position,
                     "FOREACH_BODY_SCOPE",
                     BadObject.Null,
                     BadScopeFlags.Breakable | BadScopeFlags.Continuable
                    );
        int bodyScopeStart = context.InstructionCount - 1;
        int setBreakInstruction = context.EmitEmpty();
        int setContinueInstruction = context.EmitEmpty();

        context.Emit(BadOpCode.DefVar, expression.Position, expression.LoopVariable.Text, true);
        context.Emit(BadOpCode.LoadVar, expression.Position, "~ENUMERATOR~", 0);
        context.Emit(BadOpCode.GetCurrent, expression.Position);
        context.Emit(BadOpCode.Assign, expression.Position);

        context.Compile(expression.Body);
        context.Emit(BadOpCode.DestroyScope, expression.Position);

        int continueJump = context.InstructionCount;
        context.Emit(BadOpCode.JumpRelative,
                     expression.Position,
                     loopConditionStart - context.InstructionCount - 1
                    );
        int destroyEnumeratorScope = context.InstructionCount;
        context.Emit(BadOpCode.DestroyScope, expression.Position);
        context.Emit(BadOpCode.EndLoop, expression.Position);

        context.ResolveEmpty(endJump,
                             BadOpCode.JumpRelativeIfFalse,
                             expression.Position,
                             context.InstructionCount - endJump - 1
                            );

        context.ResolveEmpty(setBreakInstruction,
                             BadOpCode.SetBreakPointer,
                             expression.Position,
                             destroyEnumeratorScope - bodyScopeStart - 1
                            );

        context.ResolveEmpty(setContinueInstruction,
                             BadOpCode.SetContinuePointer,
                             expression.Position,
                             continueJump - bodyScopeStart - 1
                            );
    }
}