using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Comparison;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadLessOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadLessOrEqualExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLessOrEqualExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LessEquals, expression.Position);
    }
}