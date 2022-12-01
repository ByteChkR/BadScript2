using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math.Assign;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadDivideAssignExpressionCompiler : BadBinaryExpressionCompiler<BadDivideAssignExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadDivideAssignExpression expression)
    {
        yield return new BadInstruction(BadOpCode.DivAssign, expression.Position);
    }
}