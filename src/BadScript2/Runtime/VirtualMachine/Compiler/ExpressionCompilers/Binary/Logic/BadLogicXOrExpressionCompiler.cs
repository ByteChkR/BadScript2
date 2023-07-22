using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

public class BadLogicXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicXOrExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicXOrExpression expression)
    {
        yield return new BadInstruction(BadOpCode.XOr, expression.Position);
    }
}