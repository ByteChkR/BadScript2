using BadScript2.Parser.Expressions.Access;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Access;

public class BadNullCoalescingExpressionCompiler : BadExpressionCompiler<BadNullCoalescingExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNullCoalescingExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Left))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.Dup, expression.Position);

		List<BadInstruction> instructions = new List<BadInstruction>();
		instructions.Add(new BadInstruction()); //Jump to end if not null
		instructions.Add(new BadInstruction(BadOpCode.Pop, expression.Position));

		foreach (BadInstruction instruction in compiler.Compile(expression.Right))
		{
			instructions.Add(instruction);
		}

		instructions[0] =
			new BadInstruction(BadOpCode.JumpRelativeIfNotNull, expression.Position, instructions.Count - 1);

		foreach (BadInstruction instruction in instructions)
		{
			yield return instruction;
		}
	}
}
