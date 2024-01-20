using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

public class BadBinaryUnpackExpressionCompiler : BadExpressionCompiler<BadBinaryUnpackExpression>
{
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