using BadScript2.Parser.Expressions.Binary.Comparison;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Comparison
{
    public class BadLessOrEqualExpressionCompiler : BadExpressionCompiler<BadLessOrEqualExpression>
    {
        public override int Compile(BadLessOrEqualExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.LessThanOrEqual, expr.Position));

            return start;
        }
    }
}