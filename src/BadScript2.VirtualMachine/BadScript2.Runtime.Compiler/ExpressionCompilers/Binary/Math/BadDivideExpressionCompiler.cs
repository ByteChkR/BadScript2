using System.Collections.Generic;

using BadScript2.Parser.Expressions.Binary.Math;
using BadScript2.VirtualMachine;

namespace BadScript2.Compiler.ExpressionCompilers.Binary.Math;

public class BadDivideExpressionCompiler : BadBinaryExpressionCompiler<BadDivideExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadDivideExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Div, expression.Position);
    }
}