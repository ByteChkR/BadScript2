using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadGreaterExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterThanExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadGreaterThanExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Greater, expression.Position);
    }
}