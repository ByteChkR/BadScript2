using System.Collections.Generic;

using BadScript2.Parser.Expressions.Variables;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Variables;

public class BadFormattedStringExpressionCompiler : BadExpressionCompiler<BadFormattedStringExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadFormattedStringExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Expressions, false))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.FormatString, expression.Position, expression.Value, expression.ExpressionCount);
    }
}