using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Logic.Assign;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadLogicAssignXOrExpressionCompiler : BadBinaryExpressionCompiler<BadLogicAssignXOrExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(
		BadCompiler compiler,
		BadLogicAssignXOrExpression expression)
	{
		yield return new BadInstruction(BadOpCode.XOrAssign, expression.Position);
	}
}
