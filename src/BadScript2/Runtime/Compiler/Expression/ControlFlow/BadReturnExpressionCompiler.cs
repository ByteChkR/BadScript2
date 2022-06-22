using BadScript2.Parser.Expressions.ControlFlow;
using BadScript2.Runtime.Objects;

namespace BadScript2.Runtime.Compiler.Expression.ControlFlow
{
    public class BadReturnExpressionCompiler : BadExpressionCompiler<BadReturnExpression>
    {
        public override int Compile(BadReturnExpression expr, BadCompilerResult result)
        {
            int start;
            if (expr.Right != null)
            {
                start = BadCompiler.CompileExpression(expr.Right, result);
            }
            else
            {
                start = result.Emit(new BadInstruction(BadOpCode.Push, expr.Position, BadObject.Null));
            }

            result.Emit(new BadInstruction(BadOpCode.Return, expr.Position));

            return start;
        }
    }
}