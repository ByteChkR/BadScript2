using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadSubtractAssignExpressionCompiler : BadBinaryExpressionCompiler<BadSubtractAssignExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(
		BadCompiler compiler,
		BadSubtractAssignExpression expression)
	{
		yield return new BadInstruction(BadOpCode.SubAssign, expression.Position);
	}
}