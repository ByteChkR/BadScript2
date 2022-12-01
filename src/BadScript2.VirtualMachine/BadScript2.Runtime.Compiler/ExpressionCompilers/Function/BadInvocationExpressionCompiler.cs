using System.Collections.Generic;

using BadScript2.Parser.Expressions.Function;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Function;

public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadInvocationExpression expression)
    {
        foreach (BadInstruction instruction in compiler.Compile(expression.Arguments, false))
        {
            yield return instruction;
        }

        foreach (BadInstruction instruction in compiler.Compile(expression.Left))
        {
            yield return instruction;
        }

        yield return new BadInstruction(BadOpCode.Invoke, expression.Position, expression.ArgumentCount);
    }
}