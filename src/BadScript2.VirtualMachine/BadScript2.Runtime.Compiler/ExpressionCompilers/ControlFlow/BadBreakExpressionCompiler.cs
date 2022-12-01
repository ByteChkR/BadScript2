using System.Collections.Generic;

using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.ControlFlow;

public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBreakExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Break, expression.Position);
    }
}