using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.Compiler.Expression.Constant;

public class BadNumberExpressionCompiler : BadExpressionCompiler<BadNumberExpression>
{
    public override int Compile(BadNumberExpression expr, BadCompilerResult result)
    {
        return result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, expr.Value));
    }
}