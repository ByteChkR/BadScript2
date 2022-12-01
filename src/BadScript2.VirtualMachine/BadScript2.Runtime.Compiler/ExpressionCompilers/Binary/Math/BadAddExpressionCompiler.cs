using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math;

public class BadAddExpressionCompiler : BadBinaryExpressionCompiler<BadAddExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Add, expression.Position);
    }
}