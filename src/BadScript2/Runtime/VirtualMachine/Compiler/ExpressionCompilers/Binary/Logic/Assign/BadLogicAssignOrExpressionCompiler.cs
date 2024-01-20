using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <summary>
/// Compiles the <see cref="BadLogicAssignOrExpression" />.
/// </summary>
public class BadLogicAssignOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignOrExpression>
{
    /// <inheritdoc />
    protected override bool EmitLeft => false;

    /// <inheritdoc />
    protected override bool EmitRight => false;

    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadLogicAssignOrExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.AddRange(compiler.Compile(expression.Left));
        instructions.Add(new BadInstruction(BadOpCode.Dup, expression.Position));
        int jump = instructions.Count;
        instructions.Add(new BadInstruction());
        instructions.AddRange(compiler.Compile(expression.Right));
        instructions.Add(new BadInstruction(BadOpCode.Assign, expression.Position));
        instructions[jump] = new BadInstruction(
            BadOpCode.JumpRelativeIfTrue,
            expression.Position,
            instructions.Count - jump - 1
        );

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}