using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadSubtractAssignExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadSubtractAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.SubAssign, expression.Position);
    }
}