using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadTernaryExpressionCompiler : BadExpressionCompiler<BadTernaryExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTernaryExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.Add(new BadInstruction()); //Jump if false

        foreach (BadInstruction instruction in compiler.Compile(expression.TrueRet))
        {
            instructions.Add(instruction);
        }

        int endJump = instructions.Count;
        instructions.Add(new BadInstruction()); //Jump
        instructions[0] =
            new BadInstruction(BadOpCode.JumpRelativeIfFalse, expression.Position, instructions.Count - 1);

        foreach (BadInstruction instruction in compiler.Compile(expression.FalseRet))
        {
            instructions.Add(instruction);
        }

        instructions[endJump] =
            new BadInstruction(BadOpCode.JumpRelative, expression.Position, instructions.Count - endJump - 1);

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}