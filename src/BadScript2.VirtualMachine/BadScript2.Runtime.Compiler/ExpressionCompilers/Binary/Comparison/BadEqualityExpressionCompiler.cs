using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadEqualityExpressionCompiler : BadBinaryExpressionCompiler<BadEqualityExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadEqualityExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Equals, expression.Position);
    }
}