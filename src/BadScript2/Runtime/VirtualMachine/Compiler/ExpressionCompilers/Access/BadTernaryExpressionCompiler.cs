using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

/// <summary>
///     Compiles the <see cref="BadTernaryExpression" />.
/// </summary>
public class BadTernaryExpressionCompiler : BadExpressionCompiler<BadTernaryExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTernaryExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        List<BadInstruction> instructions = new List<BadInstruction>
        {
            new BadInstruction(), //Jump if false
        };
        instructions.AddRange(compiler.Compile(expression.TrueRet));

        int endJump = instructions.Count;
        instructions.Add(new BadInstruction()); //Jump
        instructions[0] =
            new BadInstruction(BadOpCode.JumpRelativeIfFalse, expression.Position, instructions.Count - 1);

        instructions.AddRange(compiler.Compile(expression.FalseRet));

        instructions[endJump] =
            new BadInstruction(BadOpCode.JumpRelative, expression.Position, instructions.Count - endJump - 1);

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}