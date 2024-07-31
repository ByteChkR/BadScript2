using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <summary>
///     Compiles the <see cref="BadLogicAssignOrExpression" />.
/// </summary>
public class BadLogicAssignOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignOrExpression>
{
    /// <inheritdoc />
    protected override bool EmitLeft => false;

    /// <inheritdoc />
    protected override bool EmitRight => false;

    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicAssignOrExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Dup, expression.Position);
        int jumpPos = context.EmitEmpty();
        context.Compile(expression.Right);
        context.Emit(BadOpCode.Assign, expression.Position);

        context.ResolveEmpty(jumpPos,
                             BadOpCode.JumpRelativeIfTrue,
                             expression.Position,
                             context.InstructionCount - jumpPos - 1
                            );
    }
}