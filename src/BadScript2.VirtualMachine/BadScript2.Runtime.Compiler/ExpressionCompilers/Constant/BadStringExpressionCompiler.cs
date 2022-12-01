using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadStringExpressionCompiler : BadExpressionCompiler<BadStringExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadStringExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, (BadObject)expression.Value.Substring(1, expression.Value.Length - 2));
    }
}