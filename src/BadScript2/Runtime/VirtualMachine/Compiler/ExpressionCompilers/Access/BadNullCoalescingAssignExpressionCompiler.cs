using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

public class BadNullCoalescingAssignExpressionCompiler : BadExpressionCompiler<BadNullCoalescingAssignExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNullCoalescingAssignExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Dup, expression.Position);
        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.Add(new BadInstruction()); //Jump to end if not null
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            instructions.Add(instruction);
        }

        instructions.Add(new BadInstruction(BadOpCode.Assign, expression.Position));
        instructions[0] = new BadInstruction(BadOpCode.JumpRelativeIfNotNull, expression.Position, instructions.Count - 1);
        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}