using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

public class BadInExpressionCompiler : BadBinaryExpressionCompiler<BadInExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadInExpression expression)
	{
		

		yield return new BadInstruction(BadOpCode.HasProperty, expression.Position);
	}
}
