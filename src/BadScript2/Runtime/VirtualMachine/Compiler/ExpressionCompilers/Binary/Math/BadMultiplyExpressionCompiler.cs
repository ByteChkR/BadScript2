using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadMultiplyExpressionCompiler : BadBinaryExpressionCompiler<BadMultiplyExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(BadCompiler compiler, BadMultiplyExpression expression)
	{
		yield return new BadInstruction(BadOpCode.Mul, expression.Position);
	}
}