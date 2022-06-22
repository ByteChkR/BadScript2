using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.Compiler.Expression.Constant
{
    public class BadStringExpressionCompiler : BadExpressionCompiler<BadStringExpression>
    {
        public override int Compile(BadStringExpression expr, BadCompilerResult result)
        {
            return result.Emit(
                new BadInstruction(BadOpCode.Push, expr.Position, expr.Value.Substring(1, expr.Value.Length - 2))
            );
        }
    }
}