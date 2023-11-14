using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadLogicAssignOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignOrExpression>
{
	protected override bool EmitLeft => false;

	protected override bool EmitRight => false;

	public override IEnumerable<BadInstruction> CompileBinary(
		BadCompiler compiler,
		BadLogicAssignOrExpression expression)
	{
		List<BadInstruction> instructions = new List<BadInstruction>();
		instructions.AddRange(compiler.Compile(expression.Left));
		instructions.Add(new BadInstruction(BadOpCode.Dup, expression.Position));
		int jump = instructions.Count;
		instructions.Add(new BadInstruction());
		instructions.AddRange(compiler.Compile(expression.Right));
		instructions.Add(new BadInstruction(BadOpCode.Assign, expression.Position));
		instructions[jump] = new BadInstruction(BadOpCode.JumpRelativeIfTrue,
			expression.Position,
			instructions.Count - jump - 1);

		foreach (BadInstruction instruction in instructions)
		{
			yield return instruction;
		}
	}
}
