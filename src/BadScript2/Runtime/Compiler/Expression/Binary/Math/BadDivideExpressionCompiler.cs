using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math;

public class BadDivideExpressionCompiler : BadExpressionCompiler<BadDivideExpression>
{
    public override int Compile(BadDivideExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);

        result.Emit(new BadInstruction(BadOpCode.Div, expr.Position));

        return start;
    }
}