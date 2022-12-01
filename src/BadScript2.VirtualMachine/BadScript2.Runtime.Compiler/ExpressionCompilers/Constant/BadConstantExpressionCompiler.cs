using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadConstantExpressionCompiler : BadExpressionCompiler<BadConstantExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadConstantExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, expression.Value);
    }
}