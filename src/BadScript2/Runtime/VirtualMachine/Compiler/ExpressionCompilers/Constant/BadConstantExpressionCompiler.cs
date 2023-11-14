using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadConstantExpressionCompiler : BadExpressionCompiler<BadConstantExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadConstantExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Push, expression.Position, expression.Value);
	}
}
