using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Types;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadNewExpressionCompiler : BadExpressionCompiler<BadNewExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNewExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Right.Arguments, false))
		{
			yield return instruction;
		}

		foreach (BadInstruction instruction in compiler.Compile(expression.Right.Left))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.New, expression.Position, expression.Right.ArgumentCount);
	}
}
