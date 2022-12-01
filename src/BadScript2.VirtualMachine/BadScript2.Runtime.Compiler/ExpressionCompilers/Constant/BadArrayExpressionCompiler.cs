using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadArrayExpressionCompiler : BadExpressionCompiler<BadArrayExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.InitExpressions, false))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.ArrayInit, expression.Position, expression.Length);
    }
}