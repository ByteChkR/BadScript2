using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.Compiler.Expression.Constant;

public class BadBooleanExpressionCompiler : BadExpressionCompiler<BadBooleanExpression>
{
    public override int Compile(BadBooleanExpression expr, BadCompilerResult result)
    {
        return result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, expr.Value));
    }
}