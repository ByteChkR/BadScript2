using BadScript2.Parser.Expressions.Access;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Access
{
    public class BadNullCoalescingAssignExpressionCompiler : BadExpressionCompiler<BadNullCoalescingAssignExpression>
    {
        public override int Compile(BadNullCoalescingAssignExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
            int jumpNotNull = result.Emit(new BadInstruction(BadOpCode.JumpIfNotNull, expr.Position, BadObject.Null));
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
            BadCompiler.CompileExpression(expr.Right, result);
            result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

            int end = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));

            result.SetArgument(jumpNotNull, 0, end);

            return start;
        }
    }
}