using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Comparison
{
    public class BadGreaterThanExpressionCompiler : BadExpressionCompiler<BadGreaterThanExpression>
    {
        public override int Compile(BadGreaterThanExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.GreaterThan, expr.Position));

            return start;
        }
    }
}