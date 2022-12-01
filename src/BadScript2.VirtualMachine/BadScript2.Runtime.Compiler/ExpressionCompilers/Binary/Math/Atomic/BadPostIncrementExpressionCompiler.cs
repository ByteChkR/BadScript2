using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Atomic;

public class BadPostIncrementExpressionCompiler : BadExpressionCompiler<BadPostIncrementExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPostIncrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PostInc, expression.Position);
    }
}