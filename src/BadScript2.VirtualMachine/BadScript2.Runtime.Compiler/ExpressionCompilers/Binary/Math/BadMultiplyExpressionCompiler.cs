using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math;

public class BadMultiplyExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadMultiplyExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Mul, expression.Position);
    }
}