using BadScript2.Parser.Expressions.Variables;

namespace BadScript2.Runtime.Compiler.Expression.Variables
{
    public class BadVariableExpressionCompiler : BadExpressionCompiler<BadVariableExpression>
    {
        public override int Compile(BadVariableExpression expr, BadCompilerResult result)
        {
            int start = result.Emit(new BadInstruction(BadOpCode.PushScope, expr.Position));
            result.Emit(new BadInstruction(BadOpCode.Load, expr.Position, expr.Name));

            return start;
        }
    }
}