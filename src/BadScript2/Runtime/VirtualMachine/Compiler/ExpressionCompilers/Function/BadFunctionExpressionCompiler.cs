using BadScript2.Common.Logging;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Runtime.VirtualMachine.Compiler.ExpressionCompilers.Function;

public class BadFunctionExpressionCompiler : BadExpressionCompiler<BadFunctionExpression>
{
	public override IEnumerable<BadInstruction> Compile(BadCompiler compiler, BadFunctionExpression expression)
	{
		BadLogger.Warn($"Can not compile '{expression.GetHeader()}'",
			BadLogMask.GetMask("Compiler", "EVAL"),
			expression.Position);

		yield return new BadInstruction(BadOpCode.Eval, expression.Position, expression);
	}
}
