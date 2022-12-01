using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadMultiplyAssignExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadMultiplyAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.MulAssign, expression.Position);
    }
}