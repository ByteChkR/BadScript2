using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

public class BadNegateExpressionCompiler : BadExpressionCompiler<BadNegationExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNegationExpression expression)
	{
		foreach (BadInstruction instr in compiler.Compile(expression.Expression))
		{
			yield return instr;
		}
		yield return new BadInstruction(BadOpCode.Neg, expression.Position);
	}
}