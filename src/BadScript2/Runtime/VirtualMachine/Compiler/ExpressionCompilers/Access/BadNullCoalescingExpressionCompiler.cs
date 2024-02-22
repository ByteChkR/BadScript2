using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadNullCoalescingExpression" />.
/// </summary>
public class BadNullCoalescingExpressionCompiler : BadExpressionCompiler<BadNullCoalescingExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNullCoalescingExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Dup, expression.Position);

        List<BadInstruction> instructions = new List<BadInstruction>
        {
            new BadInstruction(), //Jump to end if not null
            new BadInstruction(BadOpCode.Pop, expression.Position),
        };
        instructions.AddRange(compiler.Compile(expression.Right));

        instructions[0] =
            new BadInstruction(BadOpCode.JumpRelativeIfNotNull, expression.Position, instructions.Count - 1);

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}