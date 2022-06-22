using BadScript2.Parser.Expressions.Binary.Math;

namespace BadScript2.Runtime.Compiler.Expression.Binary.Math;

public class BadSubtractExpressionCompiler : BadExpressionCompiler<BadSubtractExpression>
{
    public override int Compile(BadSubtractExpression expr, BadCompilerResult result)
    {
        int start = BadCompiler.CompileExpression(expr.Left, result);
        BadCompiler.CompileExpression(expr.Right, result);

        result.Emit(new BadInstruction(BadOpCode.Sub, expr.Position));

        return start;
    }
}