using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math;

public class BadSubtractExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadSubtractExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Sub, expression.Position);
    }
}