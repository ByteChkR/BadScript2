using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadAddExpressionCompiler : BadBinaryExpressionCompiler<BadAddExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadAddExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Add, expression.Position);
    }
}