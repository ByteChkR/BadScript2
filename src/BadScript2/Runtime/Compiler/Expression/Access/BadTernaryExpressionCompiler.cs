using BadScript2.Parser.Expressions.Access;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.Access
{
    public class BadTernaryExpressionCompiler : BadExpressionCompiler<BadTernaryExpression>
    {
        public override int Compile(BadTernaryExpression expr, BadCompilerResult result)
        {
            int start = BadCompiler.CompileExpression(expr.Left, result);

            int truePatch = result.Emit(new BadInstruction(BadOpCode.JumpIfTrue, expr.Position, BadObject.Null));
            BadCompiler.CompileExpression(expr.FalseRet, result);
            int endPatch = result.Emit(new BadInstruction(BadOpCode.Jump, expr.Position, BadObject.Null));
            int trueIndex = BadCompiler.CompileExpression(expr.TrueRet, result);

            int endIndex = result.Emit(new BadInstruction(BadOpCode.Nop, expr.Position));

            result.SetArgument(endPatch, 0, endIndex);
            result.SetArgument(truePatch, 0, trueIndex);

            return start;
        }
    }
}