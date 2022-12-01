using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary;

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