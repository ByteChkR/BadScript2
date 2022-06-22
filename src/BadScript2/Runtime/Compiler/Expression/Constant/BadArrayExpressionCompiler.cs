using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Constant;

namespace BadScript2.Runtime.Compiler.Expression.Constant;

public class BadArrayExpressionCompiler : BadExpressionCompiler<BadArrayExpression>
{
    public override int Compile(BadArrayExpression expr, BadCompilerResult result)
    {
        int start = -1;
        foreach (BadExpression expression in expr.InitExpressions)
        {
            int i = BadCompiler.CompileExpression(expression, result);
            if (start == -1)
            {
                start = i;
            }
        }

        int create = result.Emit(new BadInstruction(BadOpCode.CreateArray, expr.Position, expr.InitExpressions.Length));
        if (start == -1)
        {
            start = create;
        }

        return start;
    }
}