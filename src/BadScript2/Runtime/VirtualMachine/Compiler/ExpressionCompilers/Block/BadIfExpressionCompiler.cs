using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Block;
using BadScript2.Runtime.Objects;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Block;

/// <summary>
///     Compiles the <see cref="BadIfExpression" />.
/// </summary>
public class BadIfExpressionCompiler : BadExpressionCompiler<BadIfExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadIfExpression expression)
    {
        List<int> endJumps = new List<int>();
        foreach (KeyValuePair<BadExpression, BadExpression[]> branch in expression.ConditionalBranches)
        {
            context.Compile(branch.Key);
            int endJump = context.EmitEmpty();
            context.Emit(BadOpCode.CreateScope, expression.Position, "IfScope", BadObject.Null);
            context.Compile(branch.Value);
            context.Emit(BadOpCode.DestroyScope, expression.Position);
            context.ResolveEmpty(endJump, BadOpCode.JumpRelativeIfFalse, expression.Position, context.InstructionCount - endJump);
            endJumps.Add(context.EmitEmpty()); //Jump to the end of the if statement if the condition is true and the branch has been taken
        }

        if (expression.ElseBranch != null)
        {
            context.Emit(BadOpCode.CreateScope, expression.Position, "IfScope", BadObject.Null);
            context.Compile(expression.ElseBranch);
            context.Emit(BadOpCode.DestroyScope, expression.Position);
        }

        foreach (int endJump in endJumps)
        {
            int rel = context.InstructionCount - (endJump + 1);
            if (rel != 0)
            {
                context.ResolveEmpty(endJump, BadOpCode.JumpRelative, expression.Position, rel);
            }
            else
            {
                context.ResolveEmpty(endJump, BadOpCode.Nop, expression.Position, "OPTIMIZED_JUMP_REMOVAL");
            }
        }
    }
}