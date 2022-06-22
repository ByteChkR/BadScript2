using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Function;

namespace BadScript2.Runtime.Compiler.Expression.Function
{
    public class BadInvocationExpressionCompiler : BadExpressionCompiler<BadInvocationExpression>
    {
        public override int Compile(BadInvocationExpression expr, BadCompilerResult result)
        {
            int start = -1;

            foreach (BadExpression expression in expr.Arguments)
            {
                if (start == -1)
                {
                    start = BadCompiler.CompileExpression(expression, result);
                }
                else
                {
                    BadCompiler.CompileExpression(expression, result);
                }
            }

            if (start == -1)
            {
                start = BadCompiler.CompileExpression(expr.Left, result);
            }
            else
            {
                BadCompiler.CompileExpression(expr.Left, result);
            }

            result.Emit(new BadInstruction(BadOpCode.Call, expr.Position, expr.Arguments.Count()));

            return start;
        }
    }
}