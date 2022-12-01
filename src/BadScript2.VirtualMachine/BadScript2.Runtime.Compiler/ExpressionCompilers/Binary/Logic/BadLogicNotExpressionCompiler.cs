using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic;

public class BadLogicNotExpressionCompiler : BadExpressionCompiler<BadLogicNotExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadLogicNotExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Not, expression.Position);
    }
}