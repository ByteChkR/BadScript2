using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadModulusAssignExpressionCompiler : BadBinaryExpressionCompiler<BadModulusAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadModulusAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.ModAssign, expression.Position);
    }
}