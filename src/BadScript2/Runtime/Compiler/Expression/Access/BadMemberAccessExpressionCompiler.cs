using BadScript2.Parser.Expressions.Access;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Access
{
    public class BadMemberAccessExpressionCompiler : BadExpressionCompiler<BadMemberAccessExpression>
    {
        public override int Compile(BadMemberAccessExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);

            int patch = -1;
            int jumpEnd = -1;
            if (expr.NullChecked)
            {
                result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
                patch = result.Emit(new BadInstruction(BadOpCode.JumpIfNotNull, expr.Position, BadObject.Null));
                result.Emit(new BadInstruction(BadOpCode.Load, expr.Position, BadObject.Null));
                jumpEnd = result.Emit(new BadInstruction(BadOpCode.Jump, expr.Position, BadObject.Null));
            }

            int notNull = result.Emit(new BadInstruction(BadOpCode.Load, expr.Position, expr.Right.Text));

            if (expr.NullChecked)
            {
                int end = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));
                result.SetArgument(patch, 0, notNull);
                result.SetArgument(jumpEnd, 0, end);
            }

            return start;
        }
    }
}