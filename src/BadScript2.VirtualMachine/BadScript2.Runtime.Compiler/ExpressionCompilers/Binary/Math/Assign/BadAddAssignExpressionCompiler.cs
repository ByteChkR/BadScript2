using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadAddAssignExpressionCompiler : BadBinaryExpressionCompiler<BadAddAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AddAssign, expression.Position);
    }
}