using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Comparison
{
    public class BadNotEqualExpressionCompiler : BadExpressionCompiler<BadInequalityExpression>
    {
        public override int Compile(BadInequalityExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.NotEqual, expr.Position));

            return start;
        }
    }
}