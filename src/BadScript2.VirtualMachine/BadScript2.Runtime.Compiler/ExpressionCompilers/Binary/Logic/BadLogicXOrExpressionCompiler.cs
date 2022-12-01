using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic;

public class BadLogicXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicXOrExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicXOrExpression expression)
    {
        yield return new BadInstruction(BadOpCode.XOr, expression.Position);
    }
}