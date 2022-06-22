using BadScript2.Parser.Expressions.Binary.Logic;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Logic
{
    public class BadLogicXOrExpressionCompiler : BadExpressionCompiler<BadLogicXOrExpression>
    {
        public override int Compile(BadLogicXOrExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);

            result.Emit(new BadInstruction(BadOpCode.XOr, expr.Position));

            return start;
        }
    }
}