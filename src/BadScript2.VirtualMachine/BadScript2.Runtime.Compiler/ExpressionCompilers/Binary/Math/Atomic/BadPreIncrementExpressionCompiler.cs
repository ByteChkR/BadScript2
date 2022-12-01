using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Atomic;

public class BadPreIncrementExpressionCompiler : BadExpressionCompiler<BadPreIncrementExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPreIncrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PreInc, expression.Position);
    }
}