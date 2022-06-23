using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math;

public class BadAddExpressionCompiler : BadExpressionCompiler<BadAddExpression>
{
    public override int Compile(BadAddExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);

        result.Emit(new BadInstruction(BadOpCode.Add, expr.Position));

        return start;
    }
}