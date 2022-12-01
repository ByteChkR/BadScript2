using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadInequalityExpressionCompiler : BadBinaryExpressionCompiler<BadInequalityExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInequalityExpression expression)
    {
        yield return new BadInstruction(BadOpCode.NotEquals, expression.Position);
    }
}