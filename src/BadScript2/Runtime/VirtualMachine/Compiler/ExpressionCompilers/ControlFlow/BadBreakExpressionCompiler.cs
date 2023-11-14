using BadScript2.Parser.Expressions.ControlFlow;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.ControlFlow;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadBreakExpressionCompiler : BadExpressionCompiler<BadBreakExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBreakExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Break, expression.Position);
	}
}
