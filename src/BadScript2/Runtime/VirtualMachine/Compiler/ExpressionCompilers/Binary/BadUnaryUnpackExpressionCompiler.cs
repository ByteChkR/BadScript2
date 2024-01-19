using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary;

public class BadUnaryUnpackExpressionCompiler : BadExpressionCompiler<BadUnaryUnpackExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadUnaryUnpackExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Right))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.UnaryUnpack, expression.Position);
	}
}