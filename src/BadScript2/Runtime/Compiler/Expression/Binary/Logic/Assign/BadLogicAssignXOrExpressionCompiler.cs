using BadScript2.Parser.Expressions.Binary.Logic.Assign;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Logic.Assign
{
    public class BadLogicAssignXOrExpressionCompiler : BadExpressionCompiler<BadLogicAssignXOrExpression>
    {
        public override int Compile(BadLogicAssignXOrExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));

            BadCompiler.CompileExpression(expr.Right, result);

            result.Emit(new BadInstruction(BadOpCode.XOr, expr.Position));

            result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

            return start;
        }
    }
}