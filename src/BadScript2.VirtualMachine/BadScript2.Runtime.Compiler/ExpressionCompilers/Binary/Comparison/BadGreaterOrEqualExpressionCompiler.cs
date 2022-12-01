using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadGreaterOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterOrEqualExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadGreaterOrEqualExpression expression)
    {
        yield return new BadInstruction(BadOpCode.GreaterEquals, expression.Position);
    }
}