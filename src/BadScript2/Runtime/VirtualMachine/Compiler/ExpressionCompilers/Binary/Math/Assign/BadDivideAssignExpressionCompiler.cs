using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}" />
public class BadDivideAssignExpressionCompiler : BadBinaryExpressionCompiler<BadDivideAssignExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(
		BadCompiler compiler,
		BadDivideAssignExpression expression)
	{
		yield return new BadInstruction(BadOpCode.DivAssign, expression.Position);
	}
}
