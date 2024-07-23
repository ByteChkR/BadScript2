using BadScript2.Parser.Expressions.Access;
namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadNullCoalescingExpression" />.
/// </summary>
public class BadNullCoalescingExpressionCompiler : BadExpressionCompiler<BadNullCoalescingExpression>
{
    /// <inheritdoc />
    public override void Compile(BadExpressionCompileContext context, BadNullCoalescingExpression expression)
    {
        context.Compile(expression.Left);
        context.Emit(BadOpCode.Dup, expression.Position);
        int jend = context.EmitEmpty();
        context.Emit(BadOpCode.Pop, expression.Position);
        context.Compile(expression.Right);
        context.ResolveEmpty(jend, BadOpCode.JumpRelativeIfNotNull, expression.Position, context.InstructionCount - jend - 1);
    }
}