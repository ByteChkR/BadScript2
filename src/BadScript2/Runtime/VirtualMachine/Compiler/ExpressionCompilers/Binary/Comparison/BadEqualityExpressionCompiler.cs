using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Comparison;

public class BadEqualityExpressionCompiler : BadBinaryExpressionCompiler<BadEqualityExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadEqualityExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Equals, expression.Position);
	}
}
