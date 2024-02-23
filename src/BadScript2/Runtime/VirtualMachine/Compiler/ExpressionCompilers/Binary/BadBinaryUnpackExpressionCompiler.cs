using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

/// <summary>
///     Compiles the <see cref="BadBinaryUnpackExpression" />.
/// </summary>
public class BadBinaryUnpackExpressionCompiler : BadExpressionCompiler<BadBinaryUnpackExpression>
{
    /// <inheritdoc />
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBinaryUnpackExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.BinaryUnpack, expression.Position);
    }
}