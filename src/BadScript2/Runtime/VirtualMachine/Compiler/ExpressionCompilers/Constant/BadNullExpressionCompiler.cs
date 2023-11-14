using BadScript2.Parser.Expressions.Constant;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Constant;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadNullExpressionCompiler : BadExpressionCompiler<BadNullExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadNullExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Push, expression.Position, BadObject.Null);
	}
}
