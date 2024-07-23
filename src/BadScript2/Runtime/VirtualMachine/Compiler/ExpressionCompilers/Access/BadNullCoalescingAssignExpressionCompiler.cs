using BadScript2.Parser.Expressions.Access;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadNullCoalescingAssignExpression" />.
/// </summary>
public class BadNullCoalescingAssignExpressionCompiler : BadExpressionCompiler<BadNullCoalescingAssignExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNullCoalescingAssignExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Dup, expression.Position);
        int jend = context.EmitEmpty();
        context.Compile(expression.Right);
        context.Emit(BadOpCode.Assign, expression.Position);
        context.ResolveEmpty(jend, BadOpCode.JumpRelativeIfNotNull, expression.Position, context.InstructionCount - jend - 1);
    }
}