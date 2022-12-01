using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadNumberExpressionCompiler : BadExpressionCompiler<BadNumberExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNumberExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, (BadObject)expression.Value);
    }
}