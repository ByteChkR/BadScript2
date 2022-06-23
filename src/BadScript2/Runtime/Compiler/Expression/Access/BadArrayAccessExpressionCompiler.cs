using BadScript2.Parser.Expressions.Access;
using BadScript2.Runtime.Error;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Access;

public class BadArrayAccessExpressionCompiler : BadExpressionCompiler<BadArrayAccessExpression>
{
    public override int Compile(BadArrayAccessExpression expr, BadCompilerResult result)
    {
        if (expr.Arguments.Length != 1)
        {
            throw new BadRuntimeException(
                "BadArrayAccessExpressionCompiler: Only one argument is allowed",
                expr.Position
            );
        }

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

        BadCompiler.CompileExpression(expr.Arguments[0], result);
        int notNull = result.Emit(new BadInstruction(BadOpCode.Load, expr.Position));

        if (expr.NullChecked)
        {
            int end = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));
            result.SetArgument(patch, 0, notNull);
            result.SetArgument(jumpEnd, 0, end);
        }

        return start;
    }
}