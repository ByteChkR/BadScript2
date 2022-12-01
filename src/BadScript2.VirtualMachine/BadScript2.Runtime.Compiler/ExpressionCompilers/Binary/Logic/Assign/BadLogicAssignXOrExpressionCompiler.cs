using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic.Assign;

public class BadLogicAssignXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignXOrExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicAssignXOrExpression expression)
    {
        yield return new BadInstruction(BadOpCode.XOrAssign, expression.Position);
    }
}