using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

/// <summary>
/// Compiles the <see cref="BadLogicOrExpression" />.
/// </summary>
public class BadLogicOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicOrExpression>
{
    /// <inheritdoc />
    protected override bool EmitLeft => false;

    /// <inheritdoc />
    protected override bool EmitRight => false;

    /// <inheritdoc />
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicOrExpression expression)
    {
        List<BadInstruction> instructions = new List<BadInstruction>();
        instructions.AddRange(compiler.Compile(expression.Left));
        instructions.Add(new BadInstruction(BadOpCode.Dup, expression.Position));
        int jumpPos = instructions.Count;
        instructions.Add(new BadInstruction());
        instructions.Add(new BadInstruction(BadOpCode.Pop, expression.Position));
        instructions.AddRange(compiler.Compile(expression.Right));
        instructions[jumpPos] = new BadInstruction(
            BadOpCode.JumpRelativeIfTrue,
            expression.Position,
            instructions.Count - jumpPos - 1
        );

        foreach (BadInstruction instruction in instructions)
        {
            yield return instruction;
        }
    }
}