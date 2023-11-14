using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadBooleanExpressionCompiler : BadExpressionCompiler<BadBooleanExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadBooleanExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Push,
			expression.Position,
			expression.Value ? BadObject.True : BadObject.False);
	}
}
