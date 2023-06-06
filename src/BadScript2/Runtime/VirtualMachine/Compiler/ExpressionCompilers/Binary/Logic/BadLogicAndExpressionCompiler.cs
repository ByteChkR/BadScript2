using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic;

public class BadLogicAndExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAndExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadLogicAndExpression expression)
	{
		yield return new BadInstruction(BadOpCode.And, expression.Position);
	}
}
