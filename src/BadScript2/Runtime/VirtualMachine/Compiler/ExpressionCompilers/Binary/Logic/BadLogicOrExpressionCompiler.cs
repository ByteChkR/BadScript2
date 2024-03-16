using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
///     Compiles the <see cref="BadLogicOrExpression" />.
/// </summary>
public class BadLogicOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicOrExpression>
{
    /// <inheritdoc />
    protected override bool EmitLeft => false;

    /// <inheritdoc />
    protected override bool EmitRight => false;
    /// <inheritdoc />
    public override void CompileBinary(BadExpressionCompileContext context, BadLogicOrExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Dup, expression.Position);
        int jumpPos = context.EmitEmpty();
        context.Emit(BadOpCode.Pop, expression.Position);
        context.Compile(expression.Right);
        context.ResolveEmpty(jumpPos, BadOpCode.JumpRelativeIfTrue, expression.Position, context.InstructionCount - jumpPos - 1);
    }

}