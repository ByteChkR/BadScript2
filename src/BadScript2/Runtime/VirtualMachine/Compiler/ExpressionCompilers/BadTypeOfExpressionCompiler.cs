using BadScript2.Parser.Expressions;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadTypeOfExpressionCompiler : BadExpressionCompiler<BadTypeOfExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadTypeOfExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Expression))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.TypeOf, expression.Position);
	}
}
