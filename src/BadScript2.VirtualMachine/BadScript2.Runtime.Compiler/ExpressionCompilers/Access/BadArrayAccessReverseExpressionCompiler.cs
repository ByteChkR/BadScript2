using System.Collections.Generic;

using BadScript2.Parser.Expressions.Access;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Access;

public class BadArrayAccessReverseExpressionCompiler : BadExpressionCompiler<BadArrayAccessReverseExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadArrayAccessReverseExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        if (expression.NullChecked)
        {
            yield return new BadInstruction(BadOpCode.LoadArrayAccessReverseNullChecked, expression.Position, expression.ArgumentCount);
        }
        else
        {
            yield return new BadInstruction(BadOpCode.LoadArrayAccessReverse, expression.Position, expression.ArgumentCount);
        }
    }
}