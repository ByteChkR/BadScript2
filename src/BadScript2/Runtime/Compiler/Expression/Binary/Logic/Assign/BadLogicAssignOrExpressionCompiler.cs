using BadScript2.Parser.Expressions.Binary.Logic.Assign;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Logic.Assign
{
    public class BadLogicAssignOrExpressionCompiler : BadExpressionCompiler<BadLogicAssignOrExpression>
    {
        public override int Compile(BadLogicAssignOrExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
            result.Emit(new BadInstruction(BadOpCode.Dup, expr.Position));
            int patchEndTrue1 = result.Emit(new BadInstruction(BadOpCode.JumpIfTrue, expr.Position, BadObject.Null));
            BadCompiler.CompileExpression(expr.Right, result);
            int patchEndTrue2 = result.Emit(new BadInstruction(BadOpCode.JumpIfTrue, expr.Position, BadObject.Null));

            result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.False));
            int patchEndFalse = result.Emit(new BadInstruction(BadOpCode.Jump, expr.Position, BadObject.Null));


            int endTrue = result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.True));
            result.SetArgument(patchEndTrue1, 0, endTrue);
            result.SetArgument(patchEndTrue2, 0, endTrue);
            int end = result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));
            result.SetArgument(patchEndFalse, 0, end);

            return start;
        }
    }
}