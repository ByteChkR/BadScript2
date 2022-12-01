using System.Collections.Generic;

using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.ControlFlow;

public class BadContinueExpressionCompiler : BadExpressionCompiler<BadContinueExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadContinueExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Continue, expression.Position);
    }
}