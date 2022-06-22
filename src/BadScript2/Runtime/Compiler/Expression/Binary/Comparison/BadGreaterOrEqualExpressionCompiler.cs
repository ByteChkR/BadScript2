using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Comparison
{
    public class BadGreaterOrEqualExpressionCompiler : BadExpressionCompiler<BadGreaterOrEqualExpression>
    {
        public override int Compile(BadGreaterOrEqualExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.GreaterThanOrEqual, expr.Position));

            return start;
        }
    }
}