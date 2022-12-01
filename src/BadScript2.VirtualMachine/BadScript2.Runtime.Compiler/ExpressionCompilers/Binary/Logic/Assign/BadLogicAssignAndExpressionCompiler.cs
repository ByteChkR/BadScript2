using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Logic.Assign;

public class BadLogicAssignAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignAndExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicAssignAndExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AndAssign, expression.Position);
    }
}