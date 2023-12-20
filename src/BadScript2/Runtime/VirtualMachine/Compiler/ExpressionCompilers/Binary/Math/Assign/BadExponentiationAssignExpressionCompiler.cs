using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Assign;

public class BadExponentiationAssignExpressionCompiler : BadBinaryExpressionCompiler<BadExponentiationAssignExpression>
{
	public override IEnumerable<BadInstruction> CompileBinary(
		BadCompiler compiler,
		BadExponentiationAssignExpression expression)
	{
		yield return new BadInstruction(BadOpCode.ExpAssign, expression.Position);
	}
}
