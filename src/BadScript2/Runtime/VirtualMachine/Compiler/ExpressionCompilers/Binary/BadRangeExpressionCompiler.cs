using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

public class BadRangeExpressionCompiler : BadExpressionCompiler<BadRangeExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadRangeExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }


        yield return new BadInstruction(BadOpCode.Range, expression.Position);
    }
}