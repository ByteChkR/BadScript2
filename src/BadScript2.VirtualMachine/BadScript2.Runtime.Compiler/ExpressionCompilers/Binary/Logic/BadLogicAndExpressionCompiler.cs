using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic;

public class BadLogicAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAndExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicAndExpression expression)
    {
        yield return new BadInstruction(BadOpCode.And, expression.Position);
    }
}