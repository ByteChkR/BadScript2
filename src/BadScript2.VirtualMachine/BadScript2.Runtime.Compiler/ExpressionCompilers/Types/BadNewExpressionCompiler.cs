using System.Collections.Generic;

using BadScript2.Parser.Expressions.Types;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Types;

public class BadNewExpressionCompiler : BadExpressionCompiler<BadNewExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNewExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Right.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Right.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.New, expression.Position, expression.Right.ArgumentCount);
    }
}