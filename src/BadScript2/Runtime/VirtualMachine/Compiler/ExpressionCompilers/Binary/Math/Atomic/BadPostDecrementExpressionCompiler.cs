using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

public class BadPostDecrementExpressionCompiler : BadExpressionCompiler<BadPostDecrementExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPostDecrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PostDec, expression.Position);
    }
}