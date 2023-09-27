using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

public class BadLogicAssignAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignAndExpression>
{
    public override IEnumerable<BadInstruction> CompileBinary(
        BadCompiler compiler,
        BadLogicAssignAndExpression expression)
    {
        yield return new BadInstruction(BadOpCode.AndAssign, expression.Position);
    }
}