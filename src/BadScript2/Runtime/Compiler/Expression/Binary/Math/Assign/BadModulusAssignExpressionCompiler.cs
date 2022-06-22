using BadScript2.Parser.Expressions.Binary.Math.Assign;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math.Assign
{
    public class BadModulusAssignExpressionCompiler : BadExpressionCompiler<BadModulusAssignExpression>
    {
        public override int Compile(BadModulusAssignExpression expr, BadCompilerResult result)
        {
            // compile a
            int start = BadCompiler.CompileExpression(expr.Left, result); // Remains on stack as reference to Left
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For Assign
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position)); //For MOD
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.Mod, expr.Position));
            result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

            return start;
        }
    }
}