using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadDivideExpressionCompiler : BadBinaryExpressionCompiler<BadDivideExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadDivideExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Div, expression.Position);
    }
}