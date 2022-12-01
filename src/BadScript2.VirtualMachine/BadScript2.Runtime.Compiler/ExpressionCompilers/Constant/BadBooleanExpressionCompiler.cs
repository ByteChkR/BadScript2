using System.Collections.Generic;

using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Constant;

public class BadBooleanExpressionCompiler : BadExpressionCompiler<BadBooleanExpression>
{
    public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBooleanExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Push, expression.Position, expression.Value ? BadObject.True : BadObject.False);
    }
}