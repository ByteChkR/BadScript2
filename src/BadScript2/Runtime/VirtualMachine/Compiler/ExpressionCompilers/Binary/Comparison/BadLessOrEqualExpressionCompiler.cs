using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadLessOrEqualExpressionCompiler : BadBinaryExpressionCompiler<BadLessOrEqualExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLessOrEqualExpression expression)
    {
        yield return new BadInstruction(BadOpCode.LessEquals, expression.Position);
    }
}