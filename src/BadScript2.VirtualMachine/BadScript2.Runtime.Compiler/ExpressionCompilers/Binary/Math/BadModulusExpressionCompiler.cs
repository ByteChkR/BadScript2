using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math;

public class BadModulusExpressionCompiler : BadBinaryExpressionCompiler<BadModulusExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadModulusExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Mod, expression.Position);
    }
}