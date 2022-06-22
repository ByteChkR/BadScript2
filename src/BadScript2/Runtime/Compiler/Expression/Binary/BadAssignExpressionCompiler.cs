using BadScript2.Parser.Expressions.Binary;

namespace BadScript2.Runtime.Compiler.Expression.Binary;

public class BadAssignExpressionCompiler : BadExpressionCompiler<BadAssignExpression>
{
    public override int Compile(BadAssignExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);
        result.Emit(new BadInstruction(BadOpCode.Assign, expr.Position));

        return start;
    }
}