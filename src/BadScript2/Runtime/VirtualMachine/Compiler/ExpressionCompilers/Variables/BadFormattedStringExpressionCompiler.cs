using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Variables;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadFormattedStringExpressionCompiler : BadExpressionCompiler<BadFormattedStringExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadFormattedStringExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Expressions, false))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.FormatString,
			expression.Position,
			expression.Value,
			expression.ExpressionCount);
	}
}
