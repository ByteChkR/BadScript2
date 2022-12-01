using System.Collections.Generic;

using BadScript2.Parser.Expressions.Variables;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Variables;

public class BadVariableExpressionCompiler : BadExpressionCompiler<BadVariableExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadVariableExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LoadVar, expression.Position, (BadObject)expression.Name);
    }
}