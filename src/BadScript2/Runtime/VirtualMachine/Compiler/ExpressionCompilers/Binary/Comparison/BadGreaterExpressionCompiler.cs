using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadGreaterExpressionCompiler : BadBinaryExpressionCompiler<BadGreaterThanExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadGreaterThanExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Greater, expression.Position);
	}
}
