using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadLessExpressionCompiler : BadBinaryExpressionCompiler<BadLessThanExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLessThanExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Less, expression.Position);
    }
}