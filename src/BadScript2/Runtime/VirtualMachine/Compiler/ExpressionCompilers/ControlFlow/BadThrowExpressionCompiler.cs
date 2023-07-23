using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

public class BadThrowExpressionCompiler : BadExpressionCompiler<BadThrowExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadThrowExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Right))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.Throw, expression.Position);
	}
}
