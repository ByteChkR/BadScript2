using BadScript2.Parser.Expressions;
using BadScript2.Parser.Expressions.Types;

namespace BadScript2.Runtime.Compiler.Expression.Types;

public class BadNewExpressionCompiler : BadExpressionCompiler<BadNewExpression>
{
    public override int Compile(BadNewExpression expr, BadCompilerResult result)
    {
        int start = -1;
        foreach (BadExpression expression in expr.Right.Arguments)
        {
            int exprI = BadCompiler.CompileExpression(expression, result);
            if (start == -1)
            {
                start = exprI;
            }
        }

        int protoI = BadCompiler.CompileExpression(expr.Right.Left, result);
        if (start == -1)
        {
            start = protoI;
        }

        result.Emit(new BadInstruction(BadOpCode.NewObj, expr.Position, expr.Right.Arguments.Length));

        return start;
    }
}