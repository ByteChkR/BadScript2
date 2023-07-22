using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadExponentiationExpressionCompiler : BadBinaryExpressionCompiler<BadExponentiationExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadExponentiationExpression expression)
    {
        yield return new BadInstruction(BadOpCode.Exp, expression.Position);
    }
}