using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadSubtractExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadSubtractExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Sub, expression.Position);
	}
}
