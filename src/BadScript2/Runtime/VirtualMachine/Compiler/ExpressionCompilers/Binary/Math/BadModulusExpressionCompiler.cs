using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadModulusExpressionCompiler : BadBinaryExpressionCompiler<BadModulusExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadModulusExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Mod, expression.Position);
    }
}