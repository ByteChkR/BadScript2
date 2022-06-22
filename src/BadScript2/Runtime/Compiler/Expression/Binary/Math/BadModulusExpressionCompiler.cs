using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math
{
    public class BadModulusExpressionCompiler : BadExpressionCompiler<BadModulusExpression>
    {
        public override int Compile(BadModulusExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            BadCompiler.CompileExpression(expr.Right, result);

            result.Emit(new BadInstruction(BadOpCode.Mod, expr.Position));

            return start;
        }
    }
}