using BadScript2.Parser.Expressions.Binary.Math.Atomic;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Binary.Math.Atomic;

/// <inheritdoc cref="BadExpressionCompiler{T}"/>
public class BadPreDecrementExpressionCompiler : BadExpressionCompiler<BadPreDecrementExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadPreDecrementExpression expression)
	{
		foreach (BadInstruction instruction in compiler.Compile(expression.Right))
		{
			yield return instruction;
		}

		yield return new BadInstruction(BadOpCode.PreDec, expression.Position);
	}
}
