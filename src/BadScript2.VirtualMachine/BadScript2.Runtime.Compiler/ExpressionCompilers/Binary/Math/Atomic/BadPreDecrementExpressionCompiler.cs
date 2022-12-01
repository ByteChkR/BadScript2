using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Atomic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Atomic;

public class BadPreDecrementExpressionCompiler : BadExpressionCompiler<BadPreDecrementExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPreDecrementExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.PreDec, expression.Position);
    }
}