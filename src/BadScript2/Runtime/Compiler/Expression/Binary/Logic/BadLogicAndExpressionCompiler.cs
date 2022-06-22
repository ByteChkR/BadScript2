using BadScript2.Parser.Expressions.Binary.Logic;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Logic
{
    public class BadLogicAndExpressionCompiler : BadExpressionCompiler<BadLogicAndExpression>
    {
        public override int Compile(BadLogicAndExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            int patchEndFalse1 = result.Emit(new BadInstruction(BadOpCode.JumpIfFalse, expr.Position, BadObject.Null));
            BadCompiler.CompileExpression(expr.Right, result);
            int patchEndFalse2 = result.Emit(new BadInstruction(BadOpCode.JumpIfFalse, expr.Position, BadObject.Null));


            result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.True));
            int patchEnd = result.Emit(new BadInstruction(BadOpCode.Jump, expr.Position, BadObject.Null));

            int endFalse = result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.False));
            result.SetArgument(patchEndFalse1, 0, endFalse);
            result.SetArgument(patchEndFalse2, 0, endFalse);

            int end = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));
            result.SetArgument(patchEnd, 0, end);

            return start;
        }
    }
}