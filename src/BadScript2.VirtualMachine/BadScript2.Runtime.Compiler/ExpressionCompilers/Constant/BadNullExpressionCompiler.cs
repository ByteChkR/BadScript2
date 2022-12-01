using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadNullExpressionCompiler : BadExpressionCompiler<BadNullExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNullExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, BadObject.Null);
    }
}