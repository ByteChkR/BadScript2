using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadDeleteExpressionCompiler : BadExpressionCompiler<BadDeleteExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadDeleteExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.Delete, expression.Position);
	}
}
